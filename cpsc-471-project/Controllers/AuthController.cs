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
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        public AuthController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = config;
        }

        // GET: api/auth/test
        [Authorize]
        [HttpGet("test")]
        public async Task<ActionResult> Test()
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            IList<string> roles = await userManager.GetRolesAsync(user);

            return Ok(new
            {
                Id = user.Id,
                UserName = user.UserName,
                Roles = roles
            });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLogin login)
        {
            User user = await userManager.FindByNameAsync(login.UserName);

            if (user != null && await userManager.CheckPasswordAsync(user, login.Password))
            {
                JwtSecurityToken token = await GenerateToken(user);

                return Ok(new AuthResponse()
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo
                });
            }

            return Unauthorized();
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistration register)
        {
            User existingUser = await userManager.FindByNameAsync(register.UserName);
            
            if (existingUser != null)
            {
                return BadRequest("User already exists");
            }

            IdentityResult res = await userManager.CreateAsync(new User()
            {
                UserName = register.UserName,
                FirstName = register.FirstName,
                LastName = register.LastName
            }, register.Password);

            if (!res.Succeeded)
            {
                return BadRequest(res.Errors.ToString());
            }

            User user = await userManager.FindByNameAsync(register.UserName);
            JwtSecurityToken token = await GenerateToken(user);

            return Ok(new RegistrationResponse()
            {
                User = UserToDTO(user),
                Auth = new AuthResponse()
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo
                }
            });
        }

        public async Task<JwtSecurityToken> GenerateToken(User user)
        {
            List<Claim> authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (string userRole in await userManager.GetRolesAsync(user))
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            SymmetricSecurityKey authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            return new JwtSecurityToken(
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
        }

        public UserDTO UserToDTO(User user) => new UserDTO()
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber
        };
    }
}