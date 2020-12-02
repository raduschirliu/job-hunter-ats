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
    public class ExperienceController : ControllerBase
    {
        private readonly JobHunterDBContext _context;

        public ExperienceController(JobHunterDBContext context)
        {
            _context = context;
        }

        // GET: api/Experience
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExperienceDTO>>> GetExperience()
        {
            var Experience = await _context.Experience.ToListAsync();

            // NOTE: the select function here is not querying anything
            // it is simply converting the values to another format
            // i.e. the functional programming map function is named Select in C#
            return Experience.Select(x => ExperienceToDTO(x)).ToList();
        }

        // GET: api/Experience/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExperienceDTO>> GetExperience(long id)
        {
            var Experience = await _context.Experience.FindAsync(id);

            if (Experience == null)
            {
                return NotFound();
            }

            return ExperienceToDTO(Experience);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutExperience(long id, Experience Experience)
        {
            ExperienceDTO ExperienceDTO = ExperienceToDTO(Experience);
            Experience sanitizedExperience = DTOToExperience(ExperienceDTO);
            if (id != sanitizedExperience.ResumeId)
            {
                return BadRequest();
            }

            bool newExperience;
            if (!ExperienceExists(id))
            {
                newExperience = true;
                _context.Experience.Add(sanitizedExperience);
            }
            else
            {
                newExperience = false;
                _context.Entry(sanitizedExperience).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExperienceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            if (newExperience)
            {
                return CreatedAtAction("PutExperience", new { id = sanitizedExperience.ResumeId }, ExperienceDTO);
            }
            else
            {
                return AcceptedAtAction("PutExperience", new { id = sanitizedExperience.ResumeId }, ExperienceDTO);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ExperienceDTO>> PostExperience(Experience Experience)
        {
            ExperienceDTO ExperienceDTO = ExperienceToDTO(Experience);
            Experience sanitizedExperience = DTOToExperience(ExperienceDTO);
            _context.Experience.Add(sanitizedExperience);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostExperience", new { id = sanitizedExperience.ResumeId }, ExperienceDTO);
        }

        // DELETE: api/Experience/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ExperienceDTO>> DeleteExperience(long id)
        {
            var Experience = await _context.Experience.FindAsync(id);
            if (Experience == null)
            {
                return NotFound();
            }

            _context.Experience.Remove(Experience);
            await _context.SaveChangesAsync();

            return ExperienceToDTO(Experience);
        }

        private bool ExperienceExists(long id)
        {
            return _context.Experience.Any(e => e.ResumeId == id);
        }

        private static ExperienceDTO ExperienceToDTO(Experience Experience) =>
            new ExperienceDTO
            {
                ResumeId = Experience.ResumeId,
                Order = Experience.Order,
                Title = Experience.Title,
                StartDate = Experience.StartDate,
                EndDate = Experience.EndDate,
                Company = Experience.Company
            };

        private static Experience DTOToExperience(ExperienceDTO ExperienceDTO) =>
            new Experience
            {
                ResumeId = ExperienceDTO.ResumeId,
                Order = ExperienceDTO.Order,
                Title = ExperienceDTO.Title,
                StartDate = ExperienceDTO.StartDate,
                EndDate = ExperienceDTO.EndDate,
                Company = ExperienceDTO.Company,
                Resume = null
            };
    }
}
