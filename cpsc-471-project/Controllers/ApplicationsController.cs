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
    public class ApplicationsController : ControllerBase
    {
        private readonly JobHunterDBContext _context;

        public ApplicationsController(JobHunterDBContext context)
        {
            _context = context;
        }

        // GET: api/Application
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationDTO>>> GetApplications()
        {
            var app = await _context.Application.ToListAsync();

            // NOTE: the select function here is not querying anything
            // it is simply converting the values to another format
            // i.e. the functional programming map function is named Select in C#
            return app.Select(x => ApplicationToDTO(x)).ToList();
        }

        // GET: api/Application/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationDTO>> GetApplication(long id)
        {
            var application = await _context.Application.FindAsync(id);

            if (application == null)
            {
                return NotFound();
            }

            return ApplicationToDTO(application);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutApplication(long id, ApplicationDTO appDTO)
        {
            Application app = DTOToApplication(appDTO);
            // copied form from UserController, might want to follow form of CompanyController instead??
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
            _context.Application.Add(app);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetApplication", new { id = app.ApplicationId }, app);
        }

        // DELETE: api/Application/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApplicationDTO>> DeleteApplication(long id)
        {
            var app = await _context.Application.FindAsync(id);
            if (app == null)
            {
                return NotFound();
            }

            _context.Application.Remove(app);
            await _context.SaveChangesAsync();

            return ApplicationToDTO(app);
        }

        private bool ApplicationExists(long id)
        {
            return _context.Application.Any(e => e.ApplicationId == id);
        }

        private static ApplicationDTO ApplicationToDTO(Application app) =>
            new ApplicationDTO
            {
                ApplicationId = app.ApplicationId,
                JobId = app.JobId,
                DateSubmitted = app.DateSubmitted,
                Status = app.Status,
                CoverLetter = app.CoverLetter,
                //ResumeId = app.ResumeId
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
                //ResumeId = appDTO.ResumeId,
                //Resume = null
            };
    }
}
