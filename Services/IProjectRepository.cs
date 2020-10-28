using pm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_managment.Services
{
    public interface IProjectRepository : ICRUDRepository<Project>
    {
        Task<IEnumerable<Project>> FindProjectsByName(string Name);
        Task<IEnumerable<Project>> FindNotPrivateProjects();
    }
}
