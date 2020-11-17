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
    public class CommentRepository : BaseRepository, ICommentRepository
    {
        private const string commentMappingString = "id as Id, content as Content, user_id as UserId, task_id as TaskId, creation_date as CreationDate";
        private const string tableFieldsString = "id, content, user_id, task_id, creation_date";
        private const string objectFieldsString = "@Id, @Content, @UserId, @TaskId, @CreationDate";
        private const string tableName = "comments";
        public CommentRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public async Task<IEnumerable<Comment>> FindAll()
        {
            var sql = $@"SELECT {commentMappingString} FROM {tableName}";
            return await WithConnection<IEnumerable<Comment>>(
                async (connection) => await connection.QueryAsync<Comment>(sql));
        }

        public async Task<IEnumerable<Comment>> FindAll(int page, int size)
        {
            var sql = $@"SELECT {commentMappingString} FROM {tableName} ORDER BY id OFFSET {page * size} LIMIT {size}";
            return await WithConnection<IEnumerable<Comment>>(async (connection) => 
                    await connection.QueryAsync<Comment>(sql));
            
        }

        public async Task<Comment> FindById(long id)
        {
            string sql = $@"SELECT {commentMappingString} FROM {tableName} WHERE id = @id";
            return await WithConnection(async (connection) =>
            {
                return await connection.QueryFirstOrDefaultAsync<Comment>(sql, new { id = id });
            });
        }

        public async Task<IEnumerable<Comment>> FindCommentsByTaskId(long task_id)
        {
            string sql = $@"SELECT {commentMappingString} FROM {tableName} WHERE task_id = @task_id";
            return await WithConnection(async (connection) => await connection.QueryAsync<Comment>(sql, new {task_id=task_id}));
        }

        public async  System.Threading.Tasks.Task Remove(Comment entity)
        {
            string sql = $@"DELETE FROM {tableName} WHERE id = @id";

            await WithConnection(async (connection) =>
            {
                await connection.ExecuteAsync(sql, new { id = entity.Id });
            });
        }

        public async System.Threading.Tasks.Task RemoveById(long id)
        {
            string sql = $@"DELETE FROM {tableName} WHERE id = @id";

            await WithConnection(async (connection) =>
            {
                await connection.ExecuteAsync(sql, new { id });
            });
        }

        public async System.Threading.Tasks.Task Save(Comment entity)
        {
            string sql = $@"INSERT INTO {tableName}({tableFieldsString}) VALUES " +
                         $@"({objectFieldsString})";
            await WithConnection(async (connection) =>
            {
                await connection.ExecuteAsync(sql, entity);
            });
        }

        public async System.Threading.Tasks.Task Update(Comment entity)
        {
            string sql = $@"UPDATE {tableName} SET ({tableFieldsString}) = ({objectFieldsString}) WHERE id = @Id";

            await WithConnection(async (connection) =>
            {
                await connection.ExecuteAsync(sql, entity);
            });
        }
    }
}