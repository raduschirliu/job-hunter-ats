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
        // Returns all applications, depending on your role
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
                // Return all applications only for jobs they manage
                var query = from app in _context.Applications
                            join jobPost in _context.JobPosts on app.JobId equals jobPost.JobPostId
                            where jobPost.RecruiterId == user.Id
                            select app;
                applications = await query.ToListAsync();
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
        // Returns an application with a given ID, depending on your role
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationDTO>> GetApplication(long id)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            IList<string> roles = await userManager.GetRolesAsync(user);
            Application application = null;

            if (roles.Contains(UserRoles.Admin))
            {
                // Display any application for admin
                application = await _context.Applications.FindAsync(id);
            }
            else if (roles.Contains(UserRoles.Recruiter))
            {
                // Return application only for jobs they manage
                var query = from app in _context.Applications
                            join jobPost in _context.JobPosts on app.JobId equals jobPost.JobPostId
                            where app.ApplicationId == id
                            select new Application()
                            {
                                ApplicationId = app.ApplicationId,
                                CoverLetter = app.CoverLetter,
                                DateSubmitted = app.DateSubmitted,
                                JobId = app.JobId,
                                JobPost = jobPost,
                                Resume = app.Resume,
                                ResumeId = app.ResumeId
                            };
                application = await query.FirstOrDefaultAsync();

                if (application != null)
                {
                    if (application.JobPost.RecruiterId != user.Id)
                    {
                        return Unauthorized("Cannot view an application for a job that you do not manage");
                    }
                }
            }
            else
            {
                // Display your own application
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
        // Updates an existing application
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
        // Creates a new application
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
        // Deletes an existing application, depending on your role
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
                // Recruiter can only delete applications for jobs that they own
                var query = from app in _context.Applications
                            join jobPost in _context.JobPosts on app.JobId equals jobPost.JobPostId
                            where app.ApplicationId == id
                            select new Application()
                            {
                                ApplicationId = app.ApplicationId,
                                CoverLetter = app.CoverLetter,
                                DateSubmitted = app.DateSubmitted,
                                JobId = app.JobId,
                                JobPost = jobPost,
                                Resume = app.Resume,
                                ResumeId = app.ResumeId
                            };
                application = await query.FirstOrDefaultAsync();

                if (application != null)
                {
                    if (application.JobPost.RecruiterId != user.Id)
                    {
                        return Unauthorized("Cannot delete an application for a job that you do not manage");
                    }
                }
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
