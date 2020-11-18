using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using pm.Models;

namespace project_managment.Services.RepositoryImpl
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
            return await WithConnection<IEnumerable<Comment>>(
                async (connection) => await connection.QueryAsync<Comment>(sql));
        }

        public async Task<IEnumerable<Comment>> FindAll(int page, int size)
        {
            var sql = $@"SELECT {CommentMappingString} FROM {TableName} ORDER BY id OFFSET {page * size} LIMIT {size}";
            return await WithConnection<IEnumerable<Comment>>(async (connection) => 
                    await connection.QueryAsync<Comment>(sql));
            
        }

        public async Task<Comment> FindById(long id)
        {
            string sql = $@"SELECT {CommentMappingString} FROM {TableName} WHERE id = @id";
            return await WithConnection(async (connection) =>
            {
                return await connection.QueryFirstOrDefaultAsync<Comment>(sql, new { id = id });
            });
        }

        public async Task<IEnumerable<Comment>> FindCommentsByTaskId(long task_id)
        {
            string sql = $@"SELECT {CommentMappingString} FROM {TableName} WHERE task_id = @task_id";
            return await WithConnection(async (connection) => await connection.QueryAsync<Comment>(sql, new {task_id=task_id}));
        }

        public async  System.Threading.Tasks.Task Remove(Comment entity)
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

        public async System.Threading.Tasks.Task Save(Comment entity)
        {
            string sql = $@"INSERT INTO {TableName}({TableFieldsWithoutIdString}) VALUES " +
                         $@"({ObjectFieldsWithoutIdString})";
            await WithConnection(async (connection) =>
            {
                await connection.ExecuteAsync(sql, entity);
            });
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