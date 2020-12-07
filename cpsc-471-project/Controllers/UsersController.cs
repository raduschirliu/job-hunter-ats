using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using cpsc_471_project.Authentication;
using cpsc_471_project.Models;
#if DEBUG
using cpsc_471_project.Test;
#endif

namespace cpsc_471_project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly JobHunterDBContext _context;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UsersController(JobHunterDBContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        // GET: api/users
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(user => AuthController.UserToDTO(user)).ToList();
        }

        // GET: api/users/test-user
        [Authorize]
        [HttpGet("{username}")]
        public async Task<ActionResult<UserDTO>> GetUser(string username)
        {
            var user = await userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            return AuthController.UserToDTO(user);
        }

        // PATCH: api/users/{username}
        [Authorize]
        [HttpPatch("{username}")]
        public async Task<IActionResult> PatchUser(string username, UserDTO userDTO)
        {
            if (username != userDTO.UserName)
            {
                return BadRequest();
            }

            if (!UserExists(username))
            {
                return NotFound();
            }


            User authenticatedUser = await userManager.FindByNameAsync(User.Identity.Name);

            if (!(await userManager.IsInRoleAsync(authenticatedUser, UserRoles.Admin)) && !(authenticatedUser.UserName == userDTO.UserName))
            {
                return Unauthorized("Cannot edit the details of another user unless admin");
            }

            var user = await userManager.FindByNameAsync(username);

            user.Email = userDTO.Email;
            user.PhoneNumber = userDTO.PhoneNumber;
            user.FirstName = userDTO.FirstName;
            user.LastName = userDTO.LastName;

            _context.Update(user);

            await _context.SaveChangesAsync();

            return AcceptedAtAction("PatchUser", new { UserName = userDTO.UserName }, userDTO);
        }

        // DELETE: api/users/test-user
        [Authorize]
        [HttpDelete("{username}")]
        public async Task<ActionResult<UserDTO>> DeleteUser(string username)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound();
            }

            User authenticatedUser = await userManager.FindByNameAsync(User.Identity.Name);
            if (!(await userManager.IsInRoleAsync(authenticatedUser, UserRoles.Admin)) && !(authenticatedUser.UserName == username))
            {
                return Unauthorized("Cannot delete another user unless admin");
            }

            // TODO: Revoke security token

            await userManager.DeleteAsync(user);

            return AuthController.UserToDTO(user);
        }

#if DEBUG
        // POST: api/users/populatedb
        [HttpPost("PopulateDB")]
        public async Task<ActionResult<User>> PopulateDB()
        {
            await SampleData.AddSampleData(_context, userManager, roleManager);
            return Ok("Sample data has been added if there was no existing data");
        }
#endif

        private bool UserExists(string username)
        {
            return _context.Users.Any(e => e.UserName == username);
        }
    }
}
