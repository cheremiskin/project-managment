using Dapper;
using Microsoft.Extensions.Configuration;
using pm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_managment.Services
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private const string UserMappingString = "id as Id, email as Email, password as Password, birth_date as BirthDate, full_name as FullNamed, role_id as RoleId, info as Info";
        private const string TableFieldsString = "id, email, password, birth_date, full_name, role_id, info";
        private const string ObjectFieldsString = "@Id, @Email, @Password, @BirtDate, @FullName, @RoleId, @Info";
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

        public async System.Threading.Tasks.Task Save(User user)
        {
            string sql = $@"INSERT INTO users({TableFieldsString}) VALUES " +
                           $"({ObjectFieldsString})";
            await WithConnection(async (connection) =>
            {
                await connection.ExecuteAsync(sql, user);
            });
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

        public async Task<bool> EmailExists(string email)
        {
            // string sql = $@"SELECT COUNT(*) > 0 FROM users WHERE email = @email LIMIT 1";
            throw new NotImplementedException();
            // return await WithConnection(async connection => await connection.Query<bool>(sql, new {email}));
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
