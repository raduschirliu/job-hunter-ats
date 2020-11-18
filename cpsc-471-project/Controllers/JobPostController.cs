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
    public class JobPostController : ControllerBase
    {
        private readonly JobHunterDBContext _context;

        public JobPostController(JobHunterDBContext context)
        {
            _context = context;
        }

        // GET: api/JobPost
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobPost>>> GetJobPost()
        {
            return await _context.JobPost.ToListAsync();
        }

        // GET: api/JobPost/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JobPost>> GetJobPost(long id)
        {
            var post = await _context.JobPost.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutJobPost(long id, JobPost post)
        {
            // copied form from UserController, might want to follow form of CompanyController instead??
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
        public async Task<ActionResult<JobPost>> PostJobPost(JobPost post)
        {
            _context.JobPost.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetJobPost", new { id = post.JobPostId }, post);
        }

        // DELETE: api/JobPost/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<JobPost>> DeleteJobPost(long id)
        {
            var post = await _context.JobPost.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.JobPost.Remove(post);
            await _context.SaveChangesAsync();

            return post;
        }

        private bool JobPostExists(long id)
        {
            return _context.JobPost.Any(e => e.JobPostId == id);
        }
    }
}
