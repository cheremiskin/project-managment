using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DevOne.Security.Cryptography.BCrypt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using pm.Models;
using project_managment.Filters;
using project_managment.Forms;
using project_managment.Services;
using Synercoding.FormsAuthentication;
using AuthenticationOptions = project_managment.Authentication.AuthenticationOptions;

namespace project_managment.Controllers
{
    [ApiController]
    [Route("/api")]
    [Produces("application/json")]
    public class AuthenticationController : Controller
    {
        private readonly IUserRepository _userRepository;

        public AuthenticationController(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }

        [HttpPost("token")]
        [ValidateModel]
        public async Task<IActionResult> Token([FromBody] LoginForm form)
        {
            ClaimsIdentity claimsIdentity = await GetIdentity(form.Email, form.Password);
            if (claimsIdentity == null)
                return BadRequest(new {error_text = "Invalid email or password"});
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                    issuer: AuthenticationOptions.ISSUER,
                    audience: AuthenticationOptions.AUDIENCE,
                    notBefore: now,
                    claims: claimsIdentity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthenticationOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthenticationOptions.GetSymmetricSecurityKey(), 
                        AuthenticationOptions.SECURITY_ALGORITHM)
            );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt 
            };

            return Json(response);
        }

        private async Task<ClaimsIdentity> GetIdentity(string email, string password)
        {
            User user = await _userRepository.FindUserByEmail(email);
            if (user != null) //  && BCryptHelper.CheckPassword(password, user.Password)
            {
                var claims = new List<Claim> {new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email)};

                var role = await _userRepository.FindRoleByUserId(user.Id);

                claims.Add( new Claim(ClaimsIdentity.DefaultRoleClaimType, role));

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", 
                    ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            return null;
        }
        
    }
}