using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore;

using job_hunter_ats.Models;
using Microsoft.AspNetCore.Authorization;
using job_hunter_ats.Authentication;

namespace job_hunter_ats.Controllers
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
                // Return all applications only for jobs of their company
                var query = from app in _context.Applications
                            join jobPost in _context.JobPosts on app.JobId equals jobPost.JobPostId
                            join recruiter in _context.Recruiters on user.Id equals recruiter.UserId
                            where jobPost.CompanyId == recruiter.CompanyId
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
                            join recruiter in _context.Recruiters on user.Id equals recruiter.UserId
                            where app.ApplicationId == id && jobPost.CompanyId == recruiter.CompanyId
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
            if (id != appDTO.ApplicationId)
            {
                return BadRequest("Application Id is different in the URL and body of the request");
            }

            User user = await userManager.FindByNameAsync(User.Identity.Name);
            IList<string> roles = await userManager.GetRolesAsync(user);

            if((appDTO.DateSubmitted == null) || (!roles.Contains(UserRoles.Admin) && !roles.Contains(UserRoles.Recruiter)))
            {
                appDTO.DateSubmitted = DateTime.UtcNow;
            }
            Application app = DTOToApplication(appDTO);

            bool authorized = false;
            if(roles.Contains(UserRoles.Admin))
            {
                authorized = true;
            }
            else if(roles.Contains(UserRoles.Recruiter))
            {
                var query = from application in _context.Applications
                            join jobPost in _context.JobPosts on application.JobId equals jobPost.JobPostId
                            join recruiter in _context.Recruiters on user.Id equals recruiter.UserId
                            where jobPost.CompanyId == recruiter.CompanyId && application.ApplicationId == app.ApplicationId
                            select application;
                if (await query.AnyAsync())
                {
                    authorized = true;
                }
            }

            if(!authorized)
            {
                return Unauthorized("Not authorized to update this application or the application does not exist");
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

            return AcceptedAtAction("PatchApplication", new { applicationId = app.ApplicationId }, appDTO );
        }

        // POST: api/applications
        // Creates a new application
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ApplicationDTO>> PostApplication(ApplicationDTO appDTO)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            IList<string> roles = await userManager.GetRolesAsync(user);
            if ((appDTO.DateSubmitted == null) || (!roles.Contains(UserRoles.Admin)))
            {
                appDTO.DateSubmitted = DateTime.UtcNow;
            }
            Application app = DTOToApplication(appDTO);
            if (app.Status != StatusEnum.Sent)
            {
                return BadRequest("New applications must have the sent status");
            }

            Resume resume = null;
            
            if (roles.Contains(UserRoles.Admin))
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

            if (!roles.Contains(UserRoles.Admin))
            {
                var query = from jobpost in _context.JobPosts
                            where jobpost.JobPostId == app.JobId
                            select jobpost;
                JobPost jobPost = await query.FirstOrDefaultAsync();
                if (jobPost == null)
                {
                    return BadRequest("JobId not specified or job does not exist");
                }
                if (jobPost.ClosingDate != null && jobPost.ClosingDate.Date.AddSeconds(86399) <= DateTime.UtcNow)
                {
                    return Unauthorized("Cannot submit an application after the closing date");
                }
                if (app.Status != StatusEnum.Sent)
                {
                    return BadRequest("Candidates can only submit an application with a sent status");
                }
            }

            _context.Applications.Add(app);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostApplication", new { id = app.ApplicationId }, ApplicationToDTO(app));
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
                            join recruiter in _context.Recruiters on user.Id equals recruiter.UserId
                            where app.ApplicationId == id && jobPost.CompanyId == recruiter.CompanyId
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
            }
            else
            {
                // Can delete your own applications
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

        private static ApplicationDTO ApplicationToDTO(Application app)
        {
            string status = null;
            if (app.Status == StatusEnum.Sent)
            {
                status = "sent";
            }
            else if (app.Status == StatusEnum.InReview)
            {
                status = "inreview";
            }
            else if (app.Status == StatusEnum.Accepted)
            {
                status = "accepted";
            }
            else if (app.Status == StatusEnum.Rejected)
            {
                status = "rejected";
            }
            return new ApplicationDTO
            {
                ApplicationId = app.ApplicationId,
                JobId = app.JobId,
                DateSubmitted = app.DateSubmitted,
                Status = status,
                CoverLetter = app.CoverLetter,
                ResumeId = app.ResumeId
            };
        }

        private static Application DTOToApplication(ApplicationDTO appDTO)
        {
            StatusEnum statusEnum = StatusEnum.Sent;
            if (appDTO.Status == null)
            {
                statusEnum = StatusEnum.Sent;
            }
            else if (appDTO.Status.ToLowerInvariant() == "sent")
            {
                statusEnum = StatusEnum.Sent;
            }
            else if (appDTO.Status.ToLowerInvariant() == "inreview")
            {
                statusEnum = StatusEnum.InReview;
            }
            else if (appDTO.Status.ToLowerInvariant() == "accepted")
            {
                statusEnum = StatusEnum.Accepted;
            }
            else if (appDTO.Status.ToLowerInvariant() == "rejected")
            {
                statusEnum = StatusEnum.Rejected;
            }
            return new Application
            {
                ApplicationId = appDTO.ApplicationId,
                JobId = appDTO.JobId,
                JobPost = null,
                DateSubmitted = appDTO.DateSubmitted,
                Status = statusEnum,
                CoverLetter = appDTO.CoverLetter,
                ResumeId = appDTO.ResumeId,
                Resume = null
            };
        }
    }
}
