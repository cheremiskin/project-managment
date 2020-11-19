using System.Collections.Generic;
using pm.Models;
using project_managment.Data.Repositories;

namespace project_managment.Data.Services.ServiceImpl
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void RemoveById(long id)
        {
            _userRepository.RemoveById(id);
        }

        public IEnumerable<User> FindAllUsersInProject(Project project)
        {
            return _userRepository.FindUsersInProject(project.Id).Result;
        }

        public IEnumerable<User> FindAllUsersInProjectById(long projectId)
        {
            return _userRepository.FindUsersInProject(projectId).Result;
        }

        public IEnumerable<User> FindAll()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<User> FindAll(int page, int size)
        {
            return _userRepository.FindAll(page, size).Result;
        }

        public User FindById(long id)
        {
            return _userRepository.FindById(id).Result;
        }

        public User FindByEmail(string email)
        {
            return _userRepository.FindUserByEmail(email).Result;
        }

        public long Save(User user)
        {
            return _userRepository.Save(user).Result;
        }

        public void Update(User user)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(User user)
        {
            throw new System.NotImplementedException();
        }
    }
}