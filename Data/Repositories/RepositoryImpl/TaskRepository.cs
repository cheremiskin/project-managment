using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using pm.Models.Links;
using Task = pm.Models.Task;

// using System.Threading.Tasks;

namespace project_managment.Data.Repositories.RepositoryImpl
{
    public class TaskRepository : BaseRepository, ITaskRepository
    {
        private const string TaskMappingString = "id as Id, title as Title, status_id as StatusId, content as Content, project_id as ProjectId, creation_date as CreationDate, expiration_date as ExpirationDate, execution_time as ExecutionTime";
        private const string TableFieldsString = "id, title, status_id, content, project_id, creation_date, expiration_date, execution_time";
        private const string ObjectFieldsString = "@Id, @Title, @StatusId, @Content, @ProjectId, @CreationDate, @ExpirationDate, @ExecutionTime";
        private const string TableFieldsWithoutIdString = "title, status_id, content, project_id, creation_date, expiration_date, execution_time";
        private const string ObjectFieldsWithoutIdString = "@Title, @StatusId, @Content, @ProjectId, @CreationDate, @ExpirationDate, @ExecutionTime";
        private const string TableName = "tasks";

        public TaskRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public async System.Threading.Tasks.Task<IEnumerable<Task>> FindAll()
        {
            var sql = $@"SELECT {TaskMappingString} FROM {TableName}";
            return await WithConnection<IEnumerable<Task>>(
                async (connection) => await connection.QueryAsync<Task>(sql));
        }

        public async System.Threading.Tasks.Task<IEnumerable<Task>> FindAll(int page, int size)
        {
            var sql = $@"SELECT {TaskMappingString} FROM {TableName} ORDER BY id OFFSET {page * size} LIMIT {size}";
            return await WithConnection<IEnumerable<Task>>(async (connection) => 
                    await connection.QueryAsync<Task>(sql));
            
        }

        public async System.Threading.Tasks.Task<Task> FindById(long id)
        {
            string sql = $@"SELECT {TaskMappingString} FROM {TableName} WHERE id = @id";
            return await WithConnection(async (connection) => await connection.QueryFirstOrDefaultAsync<Task>(sql, new { id = id }));
        }

        public async  Task<bool> Remove(Task entity)
        {
            return await RemoveById(entity.Id);
        }

        public async Task<bool> RemoveById(long id)
        {
            string sql = $@"WITH deleted AS (DELETE FROM {TableName} WHERE id = @id) SELECT COUNT(*) > 0 FROM deleted";

            return await WithConnection(async (connection) =>
                await connection.ExecuteScalarAsync<bool>(sql, new { id })
            );
        }

        public async Task<long> Save(Task entity)
        {
            string sql = $@"INSERT INTO {TableName}({TableFieldsWithoutIdString}) VALUES " +
                         $@"({ObjectFieldsWithoutIdString}) RETURNING id";
            try
            {
                return await WithConnection(async (connection) =>
                    await connection.ExecuteScalarAsync<long>(sql, entity)
                );
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async System.Threading.Tasks.Task Update(Task entity)
        {
            string sql = $@"UPDATE {TableName} SET ({TableFieldsWithoutIdString}) = ({ObjectFieldsWithoutIdString}) WHERE id = @Id";

            await WithConnection(async (connection) =>
            {
                await connection.ExecuteAsync(sql, entity);
            });
        }

        public async Task<IEnumerable<Task>> FindAllInProjectById(long projectId)
        {
            var sql = $@"SELECT {TaskMappingString} FROM {TableName} WHERE project_id = @projectId";
            return await WithConnection(async connection => await connection.QueryAsync<Task>(sql, new {projectId = projectId}));
        }

        public async Task<TaskUser> LinkUserAndTask(long userId, long taskId)
        {
            var sql = $@"INSERT INTO task_user(user_id, task_id) VALUES (@userId, @taskId) RETURNING user_id AS UserId, task_id AS TaskId";
            try
            {
                return await WithConnection(async (connection) =>
                    await connection.QuerySingleAsync<TaskUser>(sql, new {userId = userId, taskId = taskId}));
            }
            catch (Exception ignored)
            {
                return null;
            }
            
        }

        public async Task<bool> UnlinkUserAndTask(long userId, long taskId)
        {
            var sql = "WITH deleted as (DELETE FROM task_user WHERE task_id = @taskId AND user_id = @userId RETURNING *) SELECT COUNT(*) FROM deleted";
            return await WithConnection<bool>(async (connection) =>
                await connection.ExecuteScalarAsync<bool>(sql, new {userId = userId, taskId = taskId}) 
            );
        }

        public async Task<bool> UnlinkAllUsersFromTask(long taskId)
        {
            var sql = $@"WITH deleted AS (DELETE FROM task_user WHERE task_id = @taskId RETURNING *) SELECT COUNT(*) > 0 FROM deleted";
            return await WithConnection<bool>(async (connection) =>
                await connection.ExecuteScalarAsync<bool>(sql, new {taskId = taskId}));
        }
    }
}
