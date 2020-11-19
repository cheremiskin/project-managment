using System;
using System.Collections.Generic;
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

        public async System.Threading.Tasks.Task Remove(User user)
        {
            string sql = $@"DELETE FROM {TableName} WHERE id = @id";

            await WithConnection(async (connection) =>
            {
                await connection.ExecuteAsync(sql, new { id = user.Id });
            });
        }

        public async System.Threading.Tasks.Task RemoveById(long id)
        {
            string sql = $@"DELETE FROM {TableName} WHERE id = @id";

            await WithConnection(async (connection) =>
            {
                await connection.ExecuteAsync(sql, new { id });
            });
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

        public async Task<IEnumerable<User>> FindUsersInProject(long projectId)
        {
            var sql = $@"SELECT {UserMappingString} from {TableName} where id in (SELECT user_id FROM project_user WHERE project_id = @projectId)";
            return await WithConnection<IEnumerable<User>>(async (connection) =>
                await connection.QueryAsync<User>(sql, new {projectId = projectId}));
        }

        public async System.Threading.Tasks.Task Update(User entity)
        {
            string sql = $@"UPDATE users SET email = @email, @full_name = @full_name, password = @password, info = @info " +
                           "WHERE id = @id";
            await WithConnection(async (connection) =>
            {
                await connection.ExecuteAsync(sql, new { email = entity.Email, 
                                                         full_name = entity.FullName, 
                                                         password = entity.Password,
                                                         info = entity.Info,
                                                         id = entity.Id,
                });
            });
        }
    }
}
