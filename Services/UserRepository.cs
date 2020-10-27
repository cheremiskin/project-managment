﻿using Dapper;
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
        private string userMappingString = "id Id, email Email, password Password, birth_date BirthDate, full_name FullName, rights_id RightsId";

        public UserRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<User> FindById(long id)
        {
           string sql = $@"SELECT {userMappingString} " + 
                                "FROM users WHERE id = @id";
            return await WithConnection<User>(async (connection) =>
            {
                return await connection.QueryFirstOrDefaultAsync<User>(sql, new { id = id });
            });
        } 

        public async Task<IEnumerable<User>> FindAll(int limit = 0, int offset = 0)
        {
            string sql = $@"SELECT {userMappingString} FROM users";
            return await WithConnection<IEnumerable<User>>(async (connection) =>
            {
                return await connection.QueryAsync<User>(sql);
            });
        }

        public async System.Threading.Tasks.Task Save(User user)
        {
            string sql = $@"INSERT INTO users(email, password, birth_date, full_name, rights_id) VALUES " +
                           "(@Email, @Password, @BirthDate, @FullName, @RightsId)";
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
            return await WithConnection<User>(async (connection) =>
            {
                return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
            });
        }

        public async System.Threading.Tasks.Task Update(User entity)
        {
            string sql = $@"UPDATE users SET email = @email, @full_name = @full_name, password = @password, info = @info, rights_id = @rights_id " +
                           "WHERE id = @id";
            await WithConnection(async (connection) =>
            {
                await connection.ExecuteAsync(sql, new { email = entity.Email, 
                                                         full_name = entity.FullName, 
                                                         password = entity.Password,
                                                         info = entity.Info,
                                                         rights_id = entity.RightsId,
                                                         id = entity.Id });
            });
        }
    }
}
