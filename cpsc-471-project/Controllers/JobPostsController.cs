using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cpsc_471_project.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace cpsc_471_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobPostsController : ControllerBase
    {
        private readonly JobHunterDBContext _context;

        public JobPostsController(JobHunterDBContext context)
        {
            _context = context;
        }

        // GET: api/JobPost
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobPostDTO>>> GetJobPosts()
        {
            var posts = await _context.JobPosts.ToListAsync();

            // NOTE: the select function here is not querying anything
            // it is simply converting the values to another format
            // i.e. the functional programming map function is named Select in C#
            return posts.Select(x => JobPostToDTO(x)).ToList();
        }

        // GET: api/JobPost/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JobPostDTO>> GetJobPost(long id)
        {
            var post = await _context.JobPosts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return JobPostToDTO(post);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchJobPost(long id, JobPostDTO postDTO)
        {
            JobPost post = DTOToJobPost(postDTO);
            if (id != post.JobPostId)
            {
                return BadRequest();
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

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<JobPostDTO>> PostJobPost(JobPostDTO postDTO)
        {
            JobPost post = DTOToJobPost(postDTO);
            _context.JobPosts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetJobPost", new { id = post.JobPostId }, post);
        }

        // DELETE: api/JobPost/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<JobPostDTO>> DeleteJobPost(long id)
        {
            var post = await _context.JobPosts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.JobPosts.Remove(post);
            await _context.SaveChangesAsync();

            return JobPostToDTO(post);
        }

        private bool JobPostExists(long id)
        {
            return _context.JobPosts.Any(e => e.JobPostId == id);
        }

        private static JobPostDTO JobPostToDTO(JobPost post) =>
            new JobPostDTO
            {
                JobPostId = post.JobPostId,
                CompanyId = post.CompanyId,
                Name = post.Name,
                Description = post.Description,
                ClosingDate = post.ClosingDate
            };

        private static JobPost DTOToJobPost(JobPostDTO postDTO) =>
            new JobPost
            {
                JobPostId = postDTO.JobPostId,
                CompanyId = postDTO.CompanyId,
                Company = null,
                Name = postDTO.Name,
                Description = postDTO.Description,
                ClosingDate = postDTO.ClosingDate
            };
    }
}
