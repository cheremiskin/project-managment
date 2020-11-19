using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
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

        public async  System.Threading.Tasks.Task Remove(Task entity)
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
    }
}
