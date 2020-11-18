using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Transactions;
using log4net;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using pm.Models;
using project_managment.Data.Repositories;
using project_managment.Filters;
using project_managment.Forms;
using project_managment.Services;

namespace project_managment.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        // private static readonly ILog _log = log4net.LogManager.GetLogger(typeof(UserController)); 
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }

        [HttpGet] 
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<ActionResult<IEnumerable<User>>> FindAllUsers()
        {
            var users = await _userRepository.FindAll();
            return Ok(users);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<User>> FindUserById(long id)
        {
            User user = await _userRepository.FindById(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpDelete]
        [Route("{id}")]
        public async System.Threading.Tasks.Task RemoveUserById( long id)
        {
            await _userRepository.RemoveById(id);
        }

        [HttpPut]
        [Route("{id}")]
        public async System.Threading.Tasks.Task UpdateUser(User user)
        {
            await _userRepository.Update(user);
        }

        [HttpPost]
        [Route("login")]
        public IActionResult LoginUser(LoginForm form)
        {
            
            return Ok();
        }

        [HttpPost]
        [Route("register")]
        [ValidateModel]
        public async Task<IActionResult> RegisterUser(RegistrationForm form)
        {
            User userWithEmail = await _userRepository.FindUserByEmail(form.Email);
            if (userWithEmail != null)
            {
                return BadRequest(new {error_text = "User with this email already exists"}); 
            }
            
            User user = form.ToUser();
            
            try 
            {
                await _userRepository.Save(user);
                return Ok(); // should return code 201 (created)
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
