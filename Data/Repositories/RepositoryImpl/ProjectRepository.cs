﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using pm.Models;
using Task = System.Threading.Tasks.Task;

namespace project_managment.Data.Repositories.RepositoryImpl
{
    public class ProjectRepository : BaseRepository, IProjectRepository
    {
        private const string ProjectMappingString = "id as Id, name as Name, creator_id as CreatorId, description as Description, is_private as IsPrivate, created_at as CreatedAt";
        private const string TableFieldsString = "id, name, description, created_at, creator_id, is_private";
        private const string ObjectFieldsString = "@Id, @Name, @Description, @CreatedAt, @CreatorId, @IsPrivate";
        private const string TableFieldsWithoutIdString = "name, description, created_at, creator_id, is_private";
        private const string ObjectFieldsWithoutIdString = "@Name, @Description, @CreatedAt, @CreatorId, @IsPrivate";
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
            var sql= $@"SELECT {ProjectMappingString} FROM {TableName} WHERE id = @id";
            return await WithConnection(async (connection) => await connection.QueryFirstOrDefaultAsync<Project>(sql, new { id = id }));
        }

        public async Task<IEnumerable<Project>> FindNotPrivateProjects()
        {
            var sql= $@"SELECT {ProjectMappingString} FROM {TableName} WHERE is_private = False";

            return await WithConnection(async (connection) => await connection.QueryAsync<Project>(sql));
        }

        public async Task<IEnumerable<Project>> FindProjectsByName(string name)
        {
            var sql= $@"SELECT {ProjectMappingString} FROM {TableName} WHERE name = @Name";

            return await WithConnection<IEnumerable<Project>>(async (connection) => await connection.QueryAsync<Project>(sql, new
                {
                    Name = name
                }));
        }

        public async Task<IEnumerable<Project>> FindAllNotPrivate(int page, int size)
        {
            var sql = $@"SELECT {ProjectMappingString} FROM {TableName} WHERE is_private = false ORDER BY id OFFSET {size * page} LIMIT {size}";
            return await WithConnection<IEnumerable<Project>>(async (connection) => await connection.QueryAsync<Project>(sql));
        }

        public async  System.Threading.Tasks.Task Remove(Project entity)
        {
            if (entity?.Id == null)
                throw new Exception();
            
            var sql= $@"DELETE FROM {TableName} WHERE id = @id";

            await WithConnection(async (connection) =>
            {
                await connection.ExecuteAsync(sql, new { id = entity.Id });
            });
        }

        public async System.Threading.Tasks.Task RemoveById(long id)
        {
            var sql= $@"DELETE FROM {TableName} WHERE id = @id";

            await WithConnection(async (connection) =>
            {
                await connection.ExecuteAsync(sql, new { id });
            });
        }

        public async Task<long> Save(Project entity)
        {
            if (entity == null)
                throw new Exception();
                
            var sql= $@"INSERT INTO {TableName}({TableFieldsWithoutIdString}) VALUES " +
                         $@"({ObjectFieldsWithoutIdString}) RETURNING Id";
            return await WithConnection<long>(async (connection) =>
                await connection.ExecuteScalarAsync<long>(sql, entity)
            );
            
            
            
        }

        public async System.Threading.Tasks.Task Update(Project entity)
        {
            if (entity?.Id == null)
                throw new Exception();
            List<string> tableColumns = new List<string>();
            List<string> objectFields = new List<string>();

            if (entity.IsPrivate != null)
            {
                tableColumns.Add("is_private");
                objectFields.Add("@IsPrivate");
            }

            if (entity.Description != null)
            {
                tableColumns.Add("description");
                objectFields.Add("@Description");
            }

            if (entity.Name != null)
            {
                tableColumns.Add("name");
                objectFields.Add("@Name");
            }

            if (tableColumns.Count == 0)
                return;

            string test = String.Join(", ", tableColumns);

            var sql= $@"UPDATE {TableName} SET ({String.Join(", ", tableColumns)}) = ({String.Join(",", objectFields)}) WHERE id = @Id";

            await WithConnection(async (connection) =>
            {
                await connection.ExecuteAsync(sql, entity);
            });
        }

        public async Task LinkUserAndProject(User user, Project project) // подходит ли метод под паттерн репозитория ?
        {
            if (user?.Id == null || project?.Id == null)
                throw new Exception();
            
            var sql = $@"INSERT INTO project_user(project_id, user_id) VALUES(@ProjectId, @UserId)";
            await WithConnection(async (connection) =>
            {
                await connection.ExecuteAsync(sql, new {ProjectId = project.Id, UserId = user.Id});
            });
        }
    }
}
