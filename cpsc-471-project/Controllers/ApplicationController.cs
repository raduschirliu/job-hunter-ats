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
    public class ApplicationController : ControllerBase
    {
        private readonly JobHunterDBContext _context;

        public ApplicationController(JobHunterDBContext context)
        {
            _context = context;
        }

        // GET: api/Application
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Application>>> GetApplication()
        {
            return await _context.Application.ToListAsync();
        }

        // GET: api/Application/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Application>> GetApplication(long id)
        {
            var application = await _context.Application.FindAsync(id);

            if (application == null)
            {
                return NotFound();
            }

            return application;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutApplication(long id, Application app)
        {
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
        public async Task<ActionResult<Application>> PostApplication(Application app)
        {
            _context.Application.Add(app);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetApplication", new { id = app.ApplicationId }, app);
        }

        // DELETE: api/Application/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Application>> DeleteApplication(long id)
        {
            var app = await _context.Application.FindAsync(id);
            if (app == null)
            {
                return NotFound();
            }

            _context.Application.Remove(app);
            await _context.SaveChangesAsync();

            return app;
        }

        private bool ApplicationExists(long id)
        {
            return _context.Application.Any(e => e.ApplicationId == id);
        }
    }
}
