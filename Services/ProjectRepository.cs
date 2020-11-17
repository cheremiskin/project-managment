using Dapper;
using Microsoft.Extensions.Configuration;
using pm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace project_managment.Services
{
    public class ProjectRepository : BaseRepository, IProjectRepository
    {
        private const string ProjectMappingString = "id as Id, name as Name, creator_id as CreatorId, description as Description, is_private as IsPrivate, created_at as CreatedAt";
        private const string TableFieldsString = "name, description, created_at, creator_id, is_private";
        private const string ObjectFieldsString = "@Name, @Description, @CreatedAt, @CreatorId, @IsPrivate";
        private const string TableName = "projects";

        public ProjectRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public async Task<IEnumerable<Project>> FindAll()
        {
            var sql = $@"SELECT {ProjectMappingString} FROM {TableName}";
            return await WithConnection<IEnumerable<Project>>(
                async (connection) => await connection.QueryAsync<Project>(sql));
        }

        public async Task<IEnumerable<Project>> FindAll(int page, int size)
        {
            var sql = $@"SELECT {ProjectMappingString} FROM {TableName} ORDER BY id OFFSET {page * size} LIMIT {size}";
            return await WithConnection<IEnumerable<Project>>(async (connection) => 
                    await connection.QueryAsync<Project>(sql));
            
        }

        public async Task<Project> FindById(long id)
        {
            string sql = $@"SELECT {ProjectMappingString} FROM {TableName} WHERE id = @id";
            return await WithConnection(async (connection) => await connection.QueryFirstOrDefaultAsync<Project>(sql, new { id = id }));
        }

        public async Task<IEnumerable<Project>> FindNotPrivateProjects()
        {
            string sql = $@"SELECT {ProjectMappingString} FROM {TableName} WHERE is_private = False";

            return await WithConnection(async (connection) => await connection.QueryAsync<Project>(sql));
        }

        public async Task<IEnumerable<Project>> FindProjectsByName(string Name)
        {
            string sql = $@"SELECT {ProjectMappingString} FROM {TableName} WHERE name = @Name";

            return await WithConnection<IEnumerable<Project>>(async (connection) => await connection.QueryAsync<Project>(sql, new { Name }));
        }

        public async  System.Threading.Tasks.Task Remove(Project entity)
        {
            string sql = $@"DELETE FROM {TableName} WHERE id = @id";

            await WithConnection(async (connection) =>
            {
                await connection.ExecuteAsync(sql, new { id = entity.Id });
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

        public async System.Threading.Tasks.Task Save(Project entity)
        {
            string sql = $@"INSERT INTO {TableName}({TableFieldsString}) VALUES " +
                         $@"({ObjectFieldsString})";
            await WithConnection(async (connection) =>
            {
                await connection.ExecuteAsync(sql, entity);
            });
        }

        public async System.Threading.Tasks.Task Update(Project entity)
        {
            string sql = $@"UPDATE {TableName} SET ({TableFieldsString}) = ({ObjectFieldsString}) WHERE id = @Id";

            await WithConnection(async (connection) =>
            {
                await connection.ExecuteAsync(sql, entity);
            });
        }
    }
}
