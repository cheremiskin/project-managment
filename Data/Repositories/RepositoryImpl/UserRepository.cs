using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using pm.Models;

namespace project_managment.Data.Repositories.RepositoryImpl
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private const string UserMappingString = "id as Id, email as Email, password as Password, birth_date as BirthDate, full_name as FullName, role_id as RoleId, info as Info";
        private const string TableFieldsString = "id, email, password, birth_date, full_name, role_id, info";
        private const string ObjectFieldsString = "@Id, @Email, @Password, @BirthDate, @FullName, @RoleId, @Info";
        private const string ObjectFieldsWithoutIdString = "@Email, @Password, @BirthDate, @FullName, @RoleId, @Info";
        private const string TableFieldsWithoutIdString = "email, password, birth_date, full_name, role_id, info";
        private const string TableName = "users";

        public UserRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<User> FindById(long id)
        {
           string sql = $@"SELECT {UserMappingString} " + 
                                $"FROM {TableName} WHERE id = @id";
            return await WithConnection<User>(async (connection) => await connection.QueryFirstOrDefaultAsync<User>(sql, new { id = id }));
        }

        public async Task<IEnumerable<User>> FindAll()
        {
            var sql = $@"SELECT {UserMappingString} FROM {TableName}";
            return await WithConnection<IEnumerable<User>>(
                async (connection) => await connection.QueryAsync<User>(sql));
        }

        public async Task<IEnumerable<User>> FindAll(int page, int size)
        {
            var sql = $@"SELECT {UserMappingString} FROM {TableName} ORDER BY id OFFSET {page * size} LIMIT {size}";
            return await WithConnection<IEnumerable<User>>(async (connection) => 
                    await connection.QueryAsync<User>(sql));
        }

        public async Task<long> Save(User user)
        {
            string sql = $@"INSERT INTO {TableName}({TableFieldsWithoutIdString}) VALUES " +
                           $"({ObjectFieldsWithoutIdString}) RETURNING id";
            return await WithConnection<long>(async (connection) =>  await connection.ExecuteScalarAsync<long>(sql, user) );
        }

        public async Task<bool> Remove(User user)
        {
            return await RemoveById(user.Id);
        }

        public async Task<bool> RemoveById(long id)
        {
            string sql = $@"WITH deleted AS (DELETE FROM {TableName} WHERE id = @id) SELECT COUNT(*) > 0 FROM deleted";

            return await WithConnection(async (connection) =>
                await connection.ExecuteScalarAsync<bool>(sql, new { id })
            );
        }

        public async Task<User> FindUserByEmail(string email)
        {
            string sql = $@"SELECT {UserMappingString} FROM {TableName} " +
                           "WHERE email = @Email";
            return await WithConnection<User>(async (connection) => await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email }));
        }

        public async Task<string> FindRoleByUserId(long id)
        {
            var sql = $"SELECT name FROM roles WHERE id = (SELECT role_id FROM {TableName} where id = @Id)";
            return await WithConnection<string>(async connection =>
                await connection.QueryFirstAsync<string>(sql, new {Id = id}) 
            );
        }

        public async Task<IEnumerable<User>> FindAllUsersInProject(long projectId)
        {
            var sql = $@"SELECT {UserMappingString} from {TableName} where id in (SELECT user_id FROM project_user WHERE project_id = @projectId)";
            return await WithConnection<IEnumerable<User>>(async (connection) =>
                await connection.QueryAsync<User>(sql, new {projectId = projectId}));
        }

        public async Task<IEnumerable<User>> FindAllUsersInTask(long taskId)
        {
            var sql =
                $@"SELECT {UserMappingString} from {TableName} WHERE id IN (SELECT DISTINCT user_id FROM task_user WHERE task_id = @taskId)";
            return await WithConnection(async (connection) =>
                await connection.QueryAsync<User>(sql, new {taskId}));
        }
        

        public async System.Threading.Tasks.Task Update(User entity)
        {
            var tableColumns = new List<string>();
            var objectFields = new List<string>();

            if (entity.Info != null)
            {
                tableColumns.Add("info");
                objectFields.Add("@Info");
            }

            if (entity.BirthDate != null)
            {
                tableColumns.Add("info");
                objectFields.Add("@Info");
            }

            if (entity.FullName != null)
            {
                tableColumns.Add("full_name");
                objectFields.Add("@FullName");
            }
            
            if (tableColumns.Count == 0)
                return;

            var sql = $@"UPDATE {TableName} SET ({string.Join(", ", tableColumns)}) " + 
                                                  $@"= (select {string.Join(", ", objectFields)}) where id = @Id";
            
            await WithConnection(async (connection) => await connection.ExecuteAsync(sql, entity));
        }
    }
}
