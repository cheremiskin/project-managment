using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using pm.Models;
using project_managment.Data.Repositories;
using Task = System.Threading.Tasks.Task;

namespace project_managment.Data.Services.ServiceImpl
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITaskRepository _taskRepository;

        public ProjectService(IProjectRepository projectRepository, IUserRepository userRepository, ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
        }
        
        public Task<IEnumerable<Project>> FindAll()
        {
            return _projectRepository.FindAll();
        }

        public Task<IEnumerable<Project>> FindAll(int page, int size)
        {
            return _projectRepository.FindAll(page, size);
        }

        public async Task<Project> FindById(long id)
        {
            return await _projectRepository.FindById(id);
        }


        public Task Remove(Project entity)
        {
            return _projectRepository.Remove(entity);
        }

        public Task RemoveById(long id)
        {
            return _projectRepository.RemoveById(id);
        }

        public Task<long> Save(Project entity)
        {
            return _projectRepository.Save(entity);
        }

        public Task Update(Project entity)
        {
            return _projectRepository.Update(entity);
        }

        public IEnumerable<Project> FindAllNotPrivate(int page, int size)
        {
            return _projectRepository.FindAllNotPrivate(page, size).Result;
        }

        public async Task AddTaskToProject(Project project, pm.Models.Task task)
        {
            throw new NotImplementedException();
        }

        public Task AddUserToProject(Project project, User user, IIdentity identity)
        {
            throw new System.NotImplementedException();
        }

        public async Task AddUserToProject(long projectId, long userId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveTaskFromProject(Project project, pm.Models.Task task)
        {
            // check if user has rights
            // remove task using TaskRepository
            
            throw new System.NotImplementedException();
        }

        public Task RemoveUserFromProject(Project project, User user)
        {
            
            // check if user has rights
            // remove row from project_user(project_id, user_id) if exists
            throw new System.NotImplementedException();
        }

        protected async Task<User> GetUserByIdentity(IIdentity identity)
        {
            if (identity?.Name == null)
                return null;
            return await _userRepository.FindUserByEmail(identity.Name);
        }
    }
}