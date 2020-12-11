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
    [Route("api")]
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
        [Authorize]
        [HttpGet("recruiters")]
        public async Task<ActionResult<IEnumerable<RecruiterDTO>>> GetAllRecruiters()
        {
            var query = from recruiter in _context.Recruiters
                        join recruiterUser in _context.Users on recruiter.UserId equals recruiterUser.Id
                        join company in _context.Companies on recruiter.CompanyId equals company.CompanyId
                        select new RecruiterDTO()
                        {
                            User = AuthController.UserToDTO(recruiterUser),
                            Company = CompaniesController.CompanyToDTO(company)
                        };

            return await query.ToListAsync();
        }

        // GET: api/companies/{companyId}/recruiters
        // Returns all recruiters for a specific company
        [Authorize]
        [HttpGet("companies/{companyId}/recruiters")]
        public async Task<ActionResult<IEnumerable<RecruiterDTO>>> GetRecruiters(long companyId)
        {
            var query = from recruiter in _context.Recruiters
                        join recruiterUser in _context.Users on recruiter.UserId equals recruiterUser.Id
                        join company in _context.Companies on recruiter.CompanyId equals company.CompanyId
                        where company.CompanyId == companyId
                        select new RecruiterDTO()
                        {
                            User = AuthController.UserToDTO(recruiterUser),
                            Company = CompaniesController.CompanyToDTO(company)
                        };

            return await query.ToListAsync();
        }

        // GET: api/companies/{companyId}/recruiters/{recruiterId}
        // Returns the recruiter with the input companyId and recruiterId/username
        [Authorize]
        [HttpGet("companies/{companyId}/recruiters/{recruiterId}")]
        public async Task<ActionResult<RecruiterDTO>> GetRecruiter(long companyId, string recruiterId)
        {
            var query = from recruiter in _context.Recruiters
                        join recruiterUser in _context.Users on recruiter.UserId equals recruiterUser.Id
                        join company in _context.Companies on recruiter.CompanyId equals company.CompanyId
                        where recruiter.CompanyId == companyId && recruiter.UserId == recruiterId
                        select new RecruiterDTO()
                        {
                            User = AuthController.UserToDTO(recruiterUser),
                            Company = CompaniesController.CompanyToDTO(company)
                        };

            return await query.FirstOrDefaultAsync();
        }

        // POST: api/companies/{companyId}/recruiters/{userId}
        // Makes the given user a new recruiter for the given company
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("companies/{companyId}/recruiters/{userId}")]
        public async Task<ActionResult<RecruiterDTO>> PutRecruiter(long companyId, string userId)
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

            await userManager.AddToRoleAsync(user, UserRoles.Recruiter);

            return CreatedAtAction("PutRecruiter", new { recruiter.UserId, recruiter.CompanyId }, await RecruiterToDTO(recruiter));
        }

        // DELETE: api/companies/{companyId}/recruiters/{usersId}
        // Deletes an existing recruiter from a company
        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("companies/{companyId}/recruiters/{usersId}")]
        public async Task<ActionResult<SimplifiedRecruiterDTO>> DeleteRecruiter(long companyId, string usersId)
        {
            Recruiter recruiter = await _context.Recruiters.FindAsync(usersId, companyId);

            if (recruiter == null)
            {
                return NotFound();
            }

            _context.Recruiters.Remove(recruiter);
            await _context.SaveChangesAsync();

            return Ok(new SimplifiedRecruiterDTO { UserId = recruiter.UserId, CompanyId = recruiter.CompanyId });
        }

        private async Task<RecruiterDTO> RecruiterToDTO(Recruiter recruiterInfo)
        {
            var query = from recruiter in _context.Recruiters
                        join recruiterUser in _context.Users on recruiter.UserId equals recruiterUser.Id
                        join company in _context.Companies on recruiter.CompanyId equals company.CompanyId
                        where recruiter.CompanyId == recruiterInfo.CompanyId && recruiter.UserId == recruiterInfo.UserId
                        select new RecruiterDTO()
                        {
                            User = AuthController.UserToDTO(recruiterUser),
                            Company = CompaniesController.CompanyToDTO(company)
                        };

            return await query.FirstOrDefaultAsync();
        }
    }
}