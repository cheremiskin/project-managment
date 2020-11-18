using Dapper;
using System.Data.SqlTypes;
using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace project_managment.Services
{
    public abstract class BaseRepository
    {
        // private IConfiguration configuration;
        private readonly string _connectionString;

        protected BaseRepository(IConfiguration configuration)
        {
            // this.configuration = configuration;
            this._connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        protected async Task<T> WithConnection<T>(Func<IDbConnection, Task<T>> getData)
        {
            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
                return await getData(connection);
            }
            catch (TimeoutException ex)
            {
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a SQL timeout", ex);
            }
            catch (NpgsqlException ex)
            {
                throw new Exception(
                    $"{GetType().FullName}.WithConnection() experienced a SQL exception (not a timeout)", ex);
            }

        }
        protected async Task WithConnection(Func<IDbConnection, Task> getData)
        {
            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
                await getData(connection);
            }
            catch (TimeoutException ex)
            {
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a SQL timeout", ex);
            }
            catch (NpgsqlException ex)
            {
                throw new Exception(
                    $"{GetType().FullName}.WithConnection() experienced a SQL exception (not a timeout)", ex);
            }
        }
    }
}