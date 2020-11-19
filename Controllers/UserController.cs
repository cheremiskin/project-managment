using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pm.Models;
using project_managment.Data.Dto;
using project_managment.Data.Services;
using project_managment.Forms;

namespace project_managment.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Policy = "IsUserOrAdmin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public ActionResult<List<UserDto>> GetUsers([Required, FromQuery(Name = "page")] int page,
                                                [Required, FromQuery(Name = "size")] int size)
        {
            var users = _userService.FindAll(page, size);
            return Ok(users.Select(u => new UserDto(u)));
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<UserDto> GetUser(long id)
        {
            var user = _userService.FindById(id);
            if (user == null)
                return NotFound();
            return Ok(new UserDto(user));
        }

        [HttpGet]
        [Route("me")]
        public ActionResult<UserDto> GetMe()
        {
            User user = _userService.FindByEmail(User.Identity.Name);
            if (user == null)
                return NotFound();
            return Ok(new UserDto(user));
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult PostUser(RegistrationForm form)
        {
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (User.Identity.IsAuthenticated && role != "ROLE_ADMIN")
            {
                return Forbid();
            }
            var user = form.ToUser();
            long id = 0;
            try
            {
                id = _userService.Save(user);
            }
            catch (Exception ex)
            {
                return BadRequest("error saving user");
            }

            user = _userService.FindById(id);
            return Created("", new UserDto(user));
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Policy = "IsAdmin")]
        public IActionResult DeleteUser(long id)
        {
            User user = _userService.FindByEmail(User.Identity.Name);
            if (user.Id == id)
                return BadRequest("you can't delete yourself");
            _userService.RemoveById(id);

            return Ok();
        }
        
        
        
        
    }
}
