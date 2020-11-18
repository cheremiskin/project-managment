using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using pm.Models;
using project_managment.Services.RepositoryImpl;
using Task = System.Threading.Tasks.Task;

namespace project_managment.Services.ServiceImpl
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

        public Task<Project> FindById(long id, IIdentity identity)
        {
            return _projectRepository.FindById(id);
        }

        public Task Remove(Project entity,IIdentity identity )
        {
            return _projectRepository.Remove(entity);
        }

        public Task RemoveById(long id,IIdentity identity )
        {
            return _projectRepository.RemoveById(id);
        }

        public Task Save(Project entity,IIdentity identity )
        {
            return _projectRepository.Save(entity);
        }

        public Task Update(Project entity, IIdentity identity)
        {
            return _projectRepository.Update(entity);
        }

        public async Task AddTaskToProject(Project project, pm.Models.Task task, IIdentity identity )
        {
            // check if the user has rights for operation (should pass user object?)
            // change task.ProjectId property to project.Id
            // update task using TaskRepository
            User client = await GetUserByIdentity(identity);
            if (client == null || client.Id != project.CreatorId || !identity.IsAuthenticated)
            {
                return; // forbidden 403
            }

            task.ProjectId = project.Id;
            await _taskRepository.Update(task);
        }

        public Task AddUserToProject(Project project, User user, IIdentity identity)
        {
            throw new System.NotImplementedException();
        }

        public async Task AddUserToProject(long projectId, long userId, IIdentity identity )
        {
            User user = await _userRepository.FindById(userId);
            if (user?.Id == null)
                return; // bad request
            
            Project project = await _projectRepository.FindById(projectId);
            if (project?.Id == null)
                return; // bad request
            
            User client = await GetUserByIdentity(identity);
            if (client == null || client.Id != project.CreatorId)
            {
                return; // forbidden
            }

            await _projectRepository.LinkUserAndProject(user, project);
        }

        public Task RemoveTaskFromProject(Project project, pm.Models.Task task, IIdentity identity)
        {
            // check if user has rights
            // remove task using TaskRepository
            
            throw new System.NotImplementedException();
        }

        public Task RemoveUserFromProject(Project project, User user, IIdentity identity)
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