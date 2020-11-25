using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using pm.Models;

namespace project_managment.Data.Repositories.RepositoryImpl
{
    public class CommentRepository : BaseRepository, ICommentRepository
    {
        private const string CommentMappingString = "id as Id, content as Content, user_id as UserId, task_id as TaskId, creation_date as CreationDate";
        private const string TableFieldsString = "id, content, user_id, task_id, creation_date";
        private const string ObjectFieldsString = "@Id, @Content, @UserId, @TaskId, @CreationDate";
        private const string TableFieldsWithoutIdString = "content, user_id, task_id, creation_date";
        private const string ObjectFieldsWithoutIdString = "@Content, @UserId, @TaskId, @CreationDate";
        private const string TableName = "comments";
        public CommentRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public async Task<IEnumerable<Comment>> FindAll()
        {
            var sql = $@"SELECT {CommentMappingString} FROM {TableName}";
            return await WithConnection(
                async (connection) => await connection.QueryAsync<Comment>(sql));
        }

        public async Task<IEnumerable<Comment>> FindAll(int page, int size)
        {
            var sql = $@"SELECT {CommentMappingString} FROM {TableName} ORDER BY id OFFSET {page * size} LIMIT {size}";
            return await WithConnection(async (connection) => 
                    await connection.QueryAsync<Comment>(sql));
            
        }

        public async Task<Comment> FindById(long id)
        {
            string sql = $@"SELECT {CommentMappingString} FROM {TableName} WHERE id = @id";
            return await WithConnection(async (connection) => await connection.QueryFirstOrDefaultAsync<Comment>(sql, new { id = id }));
        }

        public async Task<IEnumerable<Comment>> FindCommentsByTaskId(long taskId)
        {
            string sql = $@"SELECT {CommentMappingString} FROM {TableName} WHERE task_id = @task_id";
            return await WithConnection(async (connection) => await connection.QueryAsync<Comment>(sql, new {task_id=taskId}));
        }

        public async  Task<bool> Remove(Comment entity)
        {
            if (entity?.Id == null)
                return false;
            return await RemoveById(entity.Id);
        }

        public async Task<bool> RemoveById(long id)
        {
            string sql = $@"WITH deleted AS (DELETE FROM {TableName} WHERE id = @id) SELECT COUNT(*) > 0 FROM deleted";

            return await WithConnection(async (connection) =>
                await connection.ExecuteScalarAsync<bool>(sql, new { id })
            );
        }

        public async Task<long> Save(Comment entity)
        {
            string sql = $@"INSERT INTO {TableName}({TableFieldsWithoutIdString}) VALUES " +
                         $@"({ObjectFieldsWithoutIdString}) RETURNING id";
            return await WithConnection(async (connection) =>
                await connection.ExecuteScalarAsync<long>(sql, entity)
            );
        }

        public async System.Threading.Tasks.Task Update(Comment entity)
        {
            string sql = $@"UPDATE {TableName} SET ({TableFieldsWithoutIdString}) = ({ObjectFieldsWithoutIdString}) WHERE id = @Id";

            await WithConnection(async (connection) =>
            {
                await connection.ExecuteAsync(sql, entity);
            });
        }
    }
}