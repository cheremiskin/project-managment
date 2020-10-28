using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using pm.Models;
using project_managment.Filters;
using project_managment.Forms;
using project_managment.Services;

namespace project_managment.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
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
            User user = await userRepository.FindById(id);
            if (user == null)
                return NotFound();
            return Ok(user);
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

        [HttpPost]
        [Route("register")]
        [ValidateModel]
        public IActionResult RegisterUser(RegistrationForm form)
        {
            User user = form.ToUser();
            try 
            {
                userRepository.Save(user);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
