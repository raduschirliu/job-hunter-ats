using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using job_hunter_ats.Authentication;
using job_hunter_ats.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace job_hunter_ats.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
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

        // GET: api/auth/listroles
        [Authorize]
        [HttpGet("listroles")]
        public async Task<ActionResult> ListRoles()
        {
            // Lists roles of the current user
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            IList<string> roles = await userManager.GetRolesAsync(user);

            return Ok(new
            {
                Id = user.Id,
                UserName = user.UserName,
                Roles = roles
            });
        }

        // POST: api/auth/addrole
        [Authorize(Roles=UserRoles.Admin)]
        [HttpPost("addrole")]
        public async Task<ActionResult> AddRole(RoleChangeClass roleChange)
        {
            User user = await userManager.FindByNameAsync(roleChange.UserName);
            IdentityRole role = await roleManager.FindByNameAsync(roleChange.Role);

            if (role == null)
            {
                return NotFound("Role not found");
            }

            if (await userManager.IsInRoleAsync(user, roleChange.Role))
            {
                return BadRequest($"{roleChange.UserName} already has role {roleChange.Role}");
            }

            await userManager.AddToRoleAsync(user, roleChange.Role);

            return Ok($"Successfully added {roleChange.UserName} to role {roleChange.Role}");
        }

        // POST: api/auth/removerole
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost("removerole")]
        public async Task<ActionResult> RemoveRole(RoleChangeClass roleChange)
        {
            User user = await userManager.FindByNameAsync(roleChange.UserName);
            IdentityRole role = await roleManager.FindByNameAsync(roleChange.Role);

            if (role == null)
            {
                return NotFound("Role not found");
            }

            if (!(await userManager.IsInRoleAsync(user, roleChange.Role)))
            {
                return BadRequest($"{roleChange.UserName} does not have role {roleChange.Role}");
            }

            await userManager.RemoveFromRoleAsync(user, roleChange.Role);

            return Ok($"Successfully removed {roleChange.UserName} from role {roleChange.Role}");
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLogin login)
        {
            User user = await userManager.FindByNameAsync(login.UserName);

            if (user != null && await userManager.CheckPasswordAsync(user, login.Password))
            {
                IList<string> roles = await userManager.GetRolesAsync(user);
                JwtSecurityToken token = await GenerateToken(user);

                return Ok(new UserAuthResponse()
                {
                    User = UserToDTO(user),
                    Auth = new AuthResponse()
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        Roles = roles as List<string>,
                        Expiration = token.ValidTo
                    }
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
                Id = register.UserName,
                UserName = register.UserName,
                FirstName = register.FirstName,
                LastName = register.LastName
            }, register.Password);

            if (!res.Succeeded)
            {
                return BadRequest(res.Errors.ToString());
            }

            User user = await userManager.FindByNameAsync(register.UserName);
            await userManager.AddToRoleAsync(user, UserRoles.Candidate);
            IList<string> roles = await userManager.GetRolesAsync(user);

            JwtSecurityToken token = await GenerateToken(user);

            return Ok(new UserAuthResponse()
            {
                User = UserToDTO(user),
                Auth = new AuthResponse()
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Roles = roles as List<string>,
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
                expires: DateTime.Now.AddDays(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
        }

        public static UserDTO UserToDTO(User user) => new UserDTO()
        {
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber
        };
    }
}