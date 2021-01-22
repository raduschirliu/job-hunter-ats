using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using job_hunter_ats.Authentication;
using job_hunter_ats.Models;


namespace job_hunter_ats.Controllers
{
    public class ResumeSectionController : ControllerBase
    {
        protected UserManager<User> userManager;
        protected readonly JobHunterDBContext _context;

        public ResumeSectionController(JobHunterDBContext context, UserManager<User> userManager)
        {
            this.userManager = userManager;
            _context = context;
        }

        protected async Task<bool> ResumeAccessAuthorized(long resumeId)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            IList<string> roles = await userManager.GetRolesAsync(user);

            if (!roles.Contains(UserRoles.Admin))
            {
                var query = from resume in _context.Resumes
                            where resume.ResumeId == resumeId && resume.CandidateId == user.Id
                            select resume;
                if (!query.Any())
                {
                    return false;
                }
            }
            return true;
        }

        protected NotFoundObjectResult GenerateResumeNotFoundError(long resumeId)
        {
            return NotFound($"Resume with id {resumeId} not found");
        }
    }
}
