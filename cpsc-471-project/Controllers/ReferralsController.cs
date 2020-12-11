using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore;

using cpsc_471_project.Authentication;
using cpsc_471_project.Models;

namespace cpsc_471_project.Controllers
{
    [Route("api")]
    [ApiController]
    public class ReferralsController : ControllerBase
    {
        UserManager<User> userManager;
        private readonly JobHunterDBContext _context;

        public ReferralsController(JobHunterDBContext context, UserManager<User> userManager)
        {
            this.userManager = userManager;
            _context = context;
        }

        // GET: api/Referrals
        [Authorize]
        [HttpGet("referrals")]
        public async Task<ActionResult<IEnumerable<ReferralDTO>>> GetReferrals()
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            IList<string> roles = await userManager.GetRolesAsync(user);
            List<Referral> referrals = new List<Referral>();

            if (roles.Contains(UserRoles.Admin))
            {
                // Display all referrals for admin
                referrals = await _context.Referrals.ToListAsync();
            }
            else if (roles.Contains(UserRoles.Recruiter))
            {
                // Return all referrals only for applications associated with the jobs of their company
                var query = from referral in _context.Referrals
                            join application in _context.Applications on referral.ApplicationId equals application.ApplicationId
                            join jobPost in _context.JobPosts on application.JobId equals jobPost.JobPostId
                            join recruiter in _context.Recruiters on user.Id equals recruiter.UserId
                            where jobPost.CompanyId == recruiter.CompanyId
                            select referral;
                referrals = await query.ToListAsync();
            }
            else
            {
                // Display all of your own referrals
                var query = from referral in _context.Referrals
                            join application in _context.Applications on referral.ApplicationId equals application.ApplicationId
                            join resume in _context.Resumes on application.ResumeId equals resume.ResumeId
                            where resume.CandidateId == user.Id
                            select referral;

                referrals = await query.ToListAsync();
            }
            // NOTE: the select function here is not querying anything
            // it is simply converting the values to another format
            // i.e. the functional programming map function is named Select in C#
            return referrals.Select(x => ReferralToDTO(x)).ToList();
        }

        [Authorize]
        [HttpGet("applications/{appId}/referrals/{refId}")]
        public async Task<ActionResult<ReferralDTO>> GetReferral(long appId, long refId)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            IList<string> roles = await userManager.GetRolesAsync(user);
            Referral referralResult = null;

            if (roles.Contains(UserRoles.Admin))
            {
                // Admin can access any referral
                referralResult = await _context.Referrals.FindAsync(appId, refId);
            }
            else if (roles.Contains(UserRoles.Recruiter))
            {
                // Return referrals only for applications/jobs they manage
                var query = from referral in _context.Referrals
                            join application in _context.Applications on referral.ApplicationId equals application.ApplicationId
                            join jobPost in _context.JobPosts on application.JobId equals jobPost.JobPostId
                            join recruiter in _context.Recruiters on user.Id equals recruiter.UserId
                            where jobPost.CompanyId == recruiter.CompanyId && referral.ApplicationId == appId && referral.ReferralId == refId
                            select referral;
                referralResult = await query.FirstOrDefaultAsync();
            }
            else
            {
                // Display referrals associated with your applications
                var query = from referral in _context.Referrals
                            join application in _context.Applications on referral.ApplicationId equals application.ApplicationId
                            join resume in _context.Resumes on application.ResumeId equals resume.ResumeId
                            where resume.CandidateId == user.Id && referral.ApplicationId == appId && referral.ReferralId == refId
                            select referral;

                referralResult = await query.FirstOrDefaultAsync();
            }

            if (referralResult == null)
            {
                return NotFound();
            }

            return ReferralToDTO(referralResult);
        }

        [Authorize(Roles = UserRoles.Admin + ", " + UserRoles.Recruiter)]
        [HttpPatch("applications/{appId}/referrals/{refId}")]
        public async Task<IActionResult> PatchReferral(long appId, long refId, ReferralDTO referralDTO)
        {
            Referral referral = DTOToReferral(referralDTO);
            if (appId != referralDTO.ApplicationId || refId != referralDTO.ReferralId)
            {
                return BadRequest("One of the query params does not match the body params");
            }

            User user = await userManager.FindByNameAsync(User.Identity.Name);
            IList<string> roles = await userManager.GetRolesAsync(user);

            if (!roles.Contains(UserRoles.Admin))
            {
                var query = from queryReferral in _context.Referrals
                            join application in _context.Applications on queryReferral.ApplicationId equals application.ApplicationId
                            join jobPost in _context.JobPosts on application.JobId equals jobPost.JobPostId
                            join recruiter in _context.Recruiters on user.Id equals recruiter.UserId
                            where jobPost.CompanyId == recruiter.CompanyId
                            && queryReferral.ApplicationId == referral.ApplicationId
                            && queryReferral.ReferralId == referral.ReferralId
                            select queryReferral;
                if (!await query.AnyAsync())
                {
                    return Unauthorized("Cannot modify the referral for that application");
                }
            }

            _context.Entry(referral).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReferralExists(appId, refId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return AcceptedAtAction("PatchReferral", new { ApplicationId = referral.ApplicationId, ReferralId = referral.ReferralId }, referralDTO);
        }

        [HttpPost("applications/{appId}/referrals")] // should this potentially just be "Referrals"?
        public async Task<ActionResult<ReferralDTO>> PostReferral(ReferralDTO referralDTO)
        {
            Referral referral = DTOToReferral(referralDTO);
            _context.Referrals.Add(referral);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostReferral", new { AppliationId = referral.ApplicationId, ReferralId = referral.ReferralId }, referralDTO);

        }

        // DELETE: api/Skill/5
        [Authorize(Roles = UserRoles.Admin + ", " + UserRoles.Recruiter)]
        [HttpDelete("applications/{appId}/referrals/{refId}")]
        public async Task<ActionResult<ReferralDTO>> DeleteReferral(long appId, long refId)
        {
            var referral = await _context.Referrals.FindAsync(appId, refId);
            if (referral == null)
            {
                return NotFound();
            }
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            IList<string> roles = await userManager.GetRolesAsync(user);

            if (!roles.Contains(UserRoles.Admin))
            {
                var query = from queryReferral in _context.Referrals
                            join application in _context.Applications on queryReferral.ApplicationId equals application.ApplicationId
                            join jobPost in _context.JobPosts on application.JobId equals jobPost.JobPostId
                            join recruiter in _context.Recruiters on user.Id equals recruiter.UserId
                            where jobPost.CompanyId == recruiter.CompanyId
                            && queryReferral.ApplicationId == referral.ApplicationId
                            && queryReferral.ReferralId == referral.ReferralId
                            select queryReferral;
                if (!await query.AnyAsync())
                {
                    return Unauthorized("Cannot delete that referral");
                }
            }

            _context.Referrals.Remove(referral);
            await _context.SaveChangesAsync();

            return ReferralToDTO(referral);
        }

        private bool ReferralExists(long appId, long refId)
        {
            return _context.Referrals.Any(e => (e.ReferralId == refId && e.ApplicationId == appId));
        }

        private static ReferralDTO ReferralToDTO(Referral r) =>
            new ReferralDTO
            {
                ApplicationId = r.ApplicationId,
                ReferralId = r.ReferralId,
                Name = r.Name,
                Email = r.Email,
                Position = r.Position,
                Company = r.Company,
                Phone = r.Phone
            };

        private static Referral DTOToReferral(ReferralDTO rDTO) =>
            new Referral
            {
                ApplicationId = rDTO.ApplicationId,
                Application = null,
                ReferralId = rDTO.ReferralId,
                Name = rDTO.Name,
                Email = rDTO.Email,
                Position = rDTO.Position,
                Company = rDTO.Company,
                Phone = rDTO.Phone
            };
    }
}
