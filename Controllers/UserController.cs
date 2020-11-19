using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pm.Models;
using project_managment.Data.Dto;
using project_managment.Data.Repositories;
using project_managment.Forms;

namespace project_managment.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Policy = "IsUserOrAdmin")]
    public class UserController : ControllerBaseExt
    {
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;
        public UserController(IUserRepository userRepository, IProjectRepository projectRepository) : base(userRepository)
        {
            _userRepository = userRepository;
            _projectRepository = projectRepository;
        }

        [HttpGet]
        public ActionResult<List<UserDto>> GetUsers([Required, FromQuery(Name = "page")] int page,
                                                [Required, FromQuery(Name = "size")] int size)
        {
            var users = _userRepository.FindAll(page, size).Result;
            return Ok(users.Select(u => new UserDto(u)));
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<UserDto> GetUser(long id)
        {
            var user = _userRepository.FindById(id).Result;
            if (user == null)
                return NotFound();
            return Ok(new UserDto(user));
        }

        [HttpGet]
        [Route("me")]
        public ActionResult<UserDto> GetMe()
        {
            var user = GetClientUser();
            if (user == null)
                return NotFound();
            return Ok(new UserDto(user));
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult PostUser(RegistrationForm form)
        {
            var role = GetClientRoleClaim()?.Value;
            if (User.Identity.IsAuthenticated && role != "ROLE_ADMIN")
            {
                return Forbid();
            }
            var user = form.ToUser();
            long id = 0;
            try
            {
                id = _userRepository.Save(user).Result;
            }
            catch (Exception ex)
            {
                return BadRequest("error saving user");
            }

            user = _userRepository.FindById(id).Result;
            return Created("", new UserDto(user));
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Policy = "IsAdmin")]
        public IActionResult DeleteUser(long id)
        {
            var user = _userRepository.FindUserByEmail(User.Identity.Name).Result;
            if (user.Id == id)
                return BadRequest("you can't delete yourself");
            _userRepository.RemoveById(id);

            return Ok();
        }
    }
}
