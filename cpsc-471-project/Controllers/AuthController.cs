using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using cpsc_471_project.Authentication;
using cpsc_471_project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace cpsc_471_project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly JobHunterDBContext _context;
        private readonly IConfiguration _configuration;
        public AuthController(JobHunterDBContext context, UserManager<User> userManager, IConfiguration config)
        {
            _context = context;
            this.userManager = userManager;
            _configuration = config;
        }

        // POST: api/auth/test
        [Authorize]
        [HttpPost("test")]
        public async Task<ActionResult> Test()
        {
            return Ok("It worked cause you're logged in");
        }

        // GET: api/auth/loggedin
        [HttpGet("loggedin")]
        public async Task<ActionResult> GetLoggedIn()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Ok("Not logged in");
            }
            else
            {
                return Ok("Logged in as: " + User.Identity.Name);
            }
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLogin login)
        {
            User user = await userManager.FindByNameAsync(login.Username);

            if (user != null && await userManager.CheckPasswordAsync(user, login.Password))
            {
                List<Claim> authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (string userRole in await userManager.GetRolesAsync(user))
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                SymmetricSecurityKey authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                JwtSecurityToken token = new JwtSecurityToken(
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new AuthResponse()
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo
                });
            }

            return Unauthorized();
        }
    }
}