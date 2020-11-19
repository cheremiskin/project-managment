using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using pm.Models;
using project_managment.Data.Repositories;

namespace project_managment.Controllers
{
    public class ControllerBaseExt : ControllerBase
    {
        protected readonly IUserRepository _userRepository;

        public ControllerBaseExt(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        protected User GetClientUser()
        {
            var email = User.Identity.Name;
            return email == null ? null : _userRepository.FindUserByEmail(email).Result;
        }

        protected Claim GetClientRoleClaim()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        }
        
    }
}