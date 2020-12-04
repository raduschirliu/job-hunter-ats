using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
    public class RecruitersController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly JobHunterDBContext _context;

        public RecruitersController(JobHunterDBContext context, UserManager<User> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        // GET: api/recruiters
        // Returns all recruiter/company pairs in the system
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecruiterDTO>>> GetAllRecruiters()
        {
            var query = from recruiter in _context.Recruiters
                        join recruiterUser in _context.Users on recruiter.UserId equals recruiterUser.Id
                        join company in _context.Companies on recruiter.CompanyId equals company.CompanyId
                        select new RecruiterDTO()
                        {
                            User = recruiterUser,
                            Company = company
                        };

            return await query.ToListAsync();
        }

        // GET: api/recruiters/{companyId}
        // Returns all recruiters for a specific company
        [HttpGet("{companyId}")]
        public async Task<ActionResult<IEnumerable<RecruiterDTO>>> GetRecruiters(long companyId)
        {
            var query = from recruiter in _context.Recruiters
                        join recruiterUser in _context.Users on recruiter.UserId equals recruiterUser.Id
                        join company in _context.Companies on recruiter.CompanyId equals company.CompanyId
                        where company.CompanyId == companyId
                        select new RecruiterDTO()
                        {
                            User = recruiterUser,
                            Company = company
                        };

            return await query.ToListAsync();
        }

        // POST: api/recruiters
        // Makes the given user a new recruiter for the given company
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> PostRecruiter([FromBody] long companyId, [FromBody] string userId)
        {
            User user = await userManager.FindByIdAsync(userId);
            Company company = await _context.Companies.FindAsync(companyId);

            if (user == null)
            {
                return NotFound("User does not exist");
            }

            if (company == null)
            {
                return NotFound("Company does not exist");
            }

            Recruiter recruiter = new Recruiter()
            {
                CompanyId = company.CompanyId,
                UserId = user.Id
            };

            _context.Recruiters.Add(recruiter);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}