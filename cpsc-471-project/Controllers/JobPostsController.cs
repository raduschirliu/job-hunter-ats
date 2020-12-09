using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cpsc_471_project.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using cpsc_471_project.Authentication;

namespace cpsc_471_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobPostsController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly JobHunterDBContext _context;

        public JobPostsController(JobHunterDBContext context, UserManager<User> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        // GET: api/JobPost
        // Returns all job posts on the platform
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobPostDTO>>> GetJobPosts()
        {
            List<JobPost> posts = await _context.JobPosts.ToListAsync();

            // NOTE: the select function here is not querying anything
            // it is simply converting the values to another format
            // i.e. the functional programming map function is named Select in C#
            return posts.Select(x => JobPostToDTO(x)).ToList();
        }

        // GET: api/JobPost/{id}
        // Returns a single job post by ID
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<JobPostDTO>> GetJobPost(long id)
        {
            JobPost post = await _context.JobPosts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return JobPostToDTO(post);
        }

        // PATCH: api/JobPost/{id}
        // Updates an existing job post by ID
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Recruiter)]
        [HttpPatch("{id}")]
        public async Task<ActionResult<JobPostDTO>> PatchJobPost(long id, JobPostDTO postDTO)
        {
            JobPost post = DTOToJobPost(postDTO);

            if (id != post.JobPostId)
            {
                return BadRequest();
            }

            User user = await userManager.FindByIdAsync(User.Identity.Name);
            IList<string> roles = await userManager.GetRolesAsync(user);

            if (!roles.Contains(UserRoles.Admin))
            {
                // check if the job post is owned by a companyt hat the recruiter is part of
                var query = from jobpost in _context.JobPosts
                                        join queryRecruiter in _context.Recruiters on jobpost.CompanyId equals queryRecruiter.CompanyId
                                        where jobpost.JobPostId == post.JobPostId && queryRecruiter.UserId == user.Id
                                        select jobpost;
                if (!query.Any())
                {
                    return Unauthorized("You are not a recruiter for the currenet company specified in the job post");
                }

                Recruiter recruiter = await _context.Recruiters.FindAsync(user.Id, post.CompanyId);
                if (recruiter == null)
                {
                    return Unauthorized("You are not a recruiter for the new company specified in the job post");
                }
            }

            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobPostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return AcceptedAtAction("PatchJobPost", new { JobPostId = post.JobPostId }, postDTO);
        }

        // POST: api/JobPost
        // Create a new job post
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Recruiter)]
        [HttpPost]
        public async Task<ActionResult<JobPostDTO>> PostJobPost(JobPostDTO postDTO)
        {
            User user = await userManager.FindByIdAsync(User.Identity.Name);
            IList<string> roles = await userManager.GetRolesAsync(user);
            JobPost post = DTOToJobPost(postDTO);

            if (!roles.Contains(UserRoles.Admin))
            {
                Recruiter recruiter = await _context.Recruiters.FindAsync(user.Id, post.CompanyId);

                if (recruiter == null)
                {
                    return Unauthorized();
                }
            }

            _context.JobPosts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetJobPost", new { id = post.JobPostId }, post);
        }

        // DELETE: api/JobPost/{id}
        // Deletes an existing job post by ID
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Recruiter)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<JobPostDTO>> DeleteJobPost(long id)
        {
            JobPost post = await _context.JobPosts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            User user = await userManager.FindByIdAsync(User.Identity.Name);
            IList<string> roles = await userManager.GetRolesAsync(user);

            if (!roles.Contains(UserRoles.Admin))
            {
                Recruiter recruiter = await _context.Recruiters.FindAsync(user.Id, post.CompanyId);

                if (recruiter == null)
                {
                    return Unauthorized();
                }
            }

            _context.JobPosts.Remove(post);
            await _context.SaveChangesAsync();

            return JobPostToDTO(post);
        }

        private bool JobPostExists(long id)
        {
            return _context.JobPosts.Any(e => e.JobPostId == id);
        }

        private static JobPostDTO JobPostToDTO(JobPost post)
        {
            post.ClosingDate = post.ClosingDate.Date.AddSeconds(86399);
            return new JobPostDTO
            {
                JobPostId = post.JobPostId,
                CompanyId = post.CompanyId,
                Name = post.Name,
                Description = post.Description,
                ClosingDate = post.ClosingDate,
                Salary = post.Salary
            };
        }

        private static JobPost DTOToJobPost(JobPostDTO postDTO)
        {
            postDTO.ClosingDate = postDTO.ClosingDate.Date.AddSeconds(86399);
            return new JobPost
            {
                JobPostId = postDTO.JobPostId,
                CompanyId = postDTO.CompanyId,
                Company = null,
                Name = postDTO.Name,
                Salary = postDTO.Salary,
                Description = postDTO.Description,
                ClosingDate = postDTO.ClosingDate
            };
        }
    }
}
