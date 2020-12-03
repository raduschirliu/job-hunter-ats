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

        // GET: api/Application
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationDTO>>> GetApplications()
        {
            var app = await _context.Applications.ToListAsync();

            // NOTE: the select function here is not querying anything
            // it is simply converting the values to another format
            // i.e. the functional programming map function is named Select in C#
            return app.Select(x => ApplicationToDTO(x)).ToList();
        }

        // GET: api/Application/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationDTO>> GetApplication(long id)
        {
            var application = await _context.Applications.FindAsync(id);

            if (application == null)
            {
                return NotFound();
            }

            return ApplicationToDTO(application);
        }

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

        [HttpPost]
        public async Task<ActionResult<ApplicationDTO>> PostApplication(ApplicationDTO appDTO)
        {
            Application app = DTOToApplication(appDTO);
            app.DateSubmitted = DateTime.Now;
            _context.Applications.Add(app);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetApplication", new { id = app.ApplicationId }, app);
        }

        // DELETE: api/Application/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApplicationDTO>> DeleteApplication(long id)
        {
            var app = await _context.Applications.FindAsync(id);
            if (app == null)
            {
                return NotFound();
            }

            _context.Applications.Remove(app);
            await _context.SaveChangesAsync();

            return ApplicationToDTO(app);
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
