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
        private string userMappingString = "id as Id, email as Email, password as Password, birth_date as BirthDate, full_name as FullNamed";

        public UserRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<User> FindById(long id)
        {
           string sql = $@"SELECT {userMappingString} " + 
                                "FROM users WHERE id = @id";
            return await WithConnection<User>(async (connection) => await connection.QueryFirstOrDefaultAsync<User>(sql, new { id = id }));
        } 

        public async Task<IEnumerable<User>> FindAll(int limit = 0, int offset = 0)
        {
            string sql = $@"SELECT {userMappingString} FROM users";
            return await WithConnection<IEnumerable<User>>(async (connection) => await connection.QueryAsync<User>(sql));
        }

        public async System.Threading.Tasks.Task Save(User user)
        {
            string sql = $@"INSERT INTO users(email, password, birth_date, full_name) VALUES " +
                           "(@Email, @Password, @BirthDate, @FullName)";
            await WithConnection(async (connection) =>
            {
                await connection.ExecuteAsync(sql, user);
            });
        }

        public async System.Threading.Tasks.Task Remove(User user)
        {
            string sql = "DELETE FROM users WHERE id = @id";

            await WithConnection(async (connection) =>
            {
                await connection.ExecuteAsync(sql, new { id = user.Id });
            });
        }

        public async System.Threading.Tasks.Task RemoveById(long id)
        {
            string sql = "DELETE FROM users WHERE id = @id";

            await WithConnection(async (connection) =>
            {
                await connection.ExecuteAsync(sql, new { id });
            });
        }

        public async Task<User> FindUserByEmail(string email)
        {
            string sql = $@"SELECT {userMappingString} FROM users " +
                           "WHERE email = @Email";
            return await WithConnection<User>(async (connection) => await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email }));
        }

        public async Task<IEnumerable<string>> FindRolesById(long id)
        {
            string sql = $@"SELECT name FROM roles WHERE id IN (SELECT role_id FROM user_role WHERE user_id = @Id);";
            return await WithConnection<IEnumerable<string>>(async connection =>
                await connection.QueryAsync<string>(sql, new {Id = id}) 
            );
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
                                                         id = entity.Id });
            });
        }
    }
}
