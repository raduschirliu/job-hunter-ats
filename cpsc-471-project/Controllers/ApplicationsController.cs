using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore;

using cpsc_471_project.Models;
using Microsoft.AspNetCore.Authorization;
using cpsc_471_project.Authentication;

namespace cpsc_471_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        UserManager<User> userManager;
        private readonly JobHunterDBContext _context;

        public ApplicationsController(JobHunterDBContext context, UserManager<User> userManager)
        {
            this.userManager = userManager;
            _context = context;
        }

        // GET: api/applications
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationDTO>>> GetApplications()
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            IList<string> roles = await userManager.GetRolesAsync(user);
            List<Application> applications = new List<Application>();

            if (roles.Contains(UserRoles.Admin))
            {
                // Display all applications for admin
                applications = await _context.Applications.ToListAsync();
            }
            else if (roles.Contains(UserRoles.Recruiter))
            {
                // TODO: ?? ? ?? ?? Likely return all applications to jobs they manage? ??????
            }
            else
            {
                // Display all of your own applications
                var query = from app in _context.Applications
                            join resume in _context.Resumes on app.ResumeId equals resume.ResumeId
                            where resume.CandidateId == user.Id
                            select app;

                applications = await query.ToListAsync();
            }

            return applications.Select(app => ApplicationToDTO(app)).ToList();
        }

        // GET: api/applications/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationDTO>> GetApplication(long id)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            IList<string> roles = await userManager.GetRolesAsync(user);
            Application application = null;

            if (roles.Contains(UserRoles.Admin))
            {
                // Display all applications for admin
                application = await _context.Applications.FindAsync(id);
            }
            else if (roles.Contains(UserRoles.Recruiter))
            {
                // TODO: ?? ? ?? ?? Likely allowed to see if if only it's for a job they manage ??????
            }
            else
            {
                // Display all of your own applications
                var query = from app in _context.Applications
                            join resume in _context.Resumes on app.ResumeId equals resume.ResumeId
                            where resume.CandidateId == user.Id && app.ApplicationId == id
                            select app;

                application = await query.FirstOrDefaultAsync();
            }

            if (application == null)
            {
                return NotFound();
            }

            return ApplicationToDTO(application);
        }

        // PATCH: api/applications/{id}
        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchApplication(long id, ApplicationDTO appDTO)
        {
            Application app = DTOToApplication(appDTO);
            app.DateSubmitted = DateTime.Now;

            if (id != app.ApplicationId)
            {
                return BadRequest();
            }

            _context.Entry(app).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/applications
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ApplicationDTO>> PostApplication(ApplicationDTO appDTO)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);

            Application app = DTOToApplication(appDTO);
            app.DateSubmitted = DateTime.Now;

            Resume resume = null;
            
            if (await userManager.IsInRoleAsync(user, UserRoles.Admin))
            {
                resume = await _context.Resumes.Where(r => r.ResumeId == app.ResumeId).FirstOrDefaultAsync();
            }
            else
            {
                resume = await _context.Resumes.Where(r => r.CandidateId == user.Id && r.ResumeId == app.ResumeId).FirstOrDefaultAsync();
            }

            if (resume == null)
            {
                return NotFound("Resume does not exist");
            }

            _context.Applications.Add(app);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetApplication", new { id = app.ApplicationId }, app);
        }

        // DELETE: api/applications/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApplicationDTO>> DeleteApplication(long id)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            IList<string> roles = await userManager.GetRolesAsync(user);
            Application application = null;

            if (roles.Contains(UserRoles.Admin))
            {
                // Admin can delete any application
                application = await _context.Applications.FindAsync(id);
            }
            else if (roles.Contains(UserRoles.Recruiter))
            {
                // TODO: ?? ? ?? ?? Likely allowed to delete if only it's for a job they manage ??????
            }
            else
            {
                // Display all of your own applications
                var query = from app in _context.Applications
                            join resume in _context.Resumes on app.ResumeId equals resume.ResumeId
                            where resume.CandidateId == user.Id && app.ApplicationId == id
                            select app;

                application = await query.FirstOrDefaultAsync();
            }

            if (application == null)
            {
                return NotFound();
            }

            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();

            return ApplicationToDTO(application);
        }

        private bool ApplicationExists(long id)
        {
            return _context.Applications.Any(e => e.ApplicationId == id);
        }

        private static ApplicationDTO ApplicationToDTO(Application app) =>
            new ApplicationDTO
            {
                ApplicationId = app.ApplicationId,
                JobId = app.JobId,
                DateSubmitted = app.DateSubmitted,
                Status = app.Status,
                CoverLetter = app.CoverLetter,
                ResumeId = app.ResumeId
            };

        private static Application DTOToApplication(ApplicationDTO appDTO) =>
            new Application
            {
                ApplicationId = appDTO.ApplicationId,
                JobId = appDTO.JobId,
                JobPost = null,
                DateSubmitted = appDTO.DateSubmitted,
                Status = appDTO.Status,
                CoverLetter = appDTO.CoverLetter,
                ResumeId = appDTO.ResumeId,
                Resume = null
            };
    }
}
