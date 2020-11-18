using cpsc_471_project.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cpsc_471_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResumeController : Controller
    {
        private readonly JobHunterDBContext _context;

        public ResumeController(JobHunterDBContext context)
        {
            _context = context;
        }

        // GET: api/Resume/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyDTO>> GetResume(long id)
        {
            Resume resume = await _context.Resume.FindAsync(id);

            // TODO: Add auth check to make sure you can't get someone else's resumes unless admin

            if (resume == null)
            {
                return NotFound();
            }

            return resume;
        }
    }
}
