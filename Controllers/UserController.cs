using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using pm.Models;
using project_managment.Services;

namespace project_managment.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : Controller
    {
        private IUserRepository userRepository;
        public UserController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [HttpGet] 
        public async Task<ActionResult<IEnumerable<User>>> FindAllUsers()
        {
            var users = await userRepository.FindAll();
            return Ok(users);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<User>> FindUserById(long id)
        {
            return Ok(await userRepository.FindById(id));
        }

        [HttpPost]
        public async System.Threading.Tasks.Task SaveUser(User user)
        {
            await userRepository.Save(user);
        }

        [HttpDelete]
        [Route("{id}")]
        public async System.Threading.Tasks.Task RemoveUserById( long id)
        {
            await userRepository.RemoveById(id);
        }

        [HttpPut]
        [Route("{id}")]
        public async System.Threading.Tasks.Task UpdateUser(User user)
        {
            await userRepository.Update(user);
        }
    }
}
