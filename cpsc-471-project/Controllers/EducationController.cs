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
    public class EducationController : ControllerBase
    {
        private readonly JobHunterDBContext _context;

        public EducationController(JobHunterDBContext context)
        {
            _context = context;
        }

        // GET: api/Education
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EducationDTO>>> GetEducation()
        {
            var Education = await _context.Education.ToListAsync();

            // NOTE: the select function here is not querying anything
            // it is simply converting the values to another format
            // i.e. the functional programming map function is named Select in C#
            return Education.Select(x => EducationToDTO(x)).ToList();
        }

        // GET: api/Education/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EducationDTO>> GetEducation(long id)
        {
            var Education = await _context.Education.FindAsync(id);

            if (Education == null)
            {
                return NotFound();
            }

            return EducationToDTO(Education);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEducation(long id, Education Education)
        {
            EducationDTO EducationDTO = EducationToDTO(Education);
            Education sanitizedEducation = DTOToEducation(EducationDTO);
            if (id != sanitizedEducation.ResumeId)
            {
                return BadRequest();
            }

            bool newEducation;
            if (!EducationExists(id))
            {
                newEducation = true;
                _context.Education.Add(sanitizedEducation);
            }
            else
            {
                newEducation = false;
                _context.Entry(sanitizedEducation).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EducationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            if (newEducation)
            {
                return CreatedAtAction("PutEducation", new { id = sanitizedEducation.ResumeId }, EducationDTO);
            }
            else
            {
                return AcceptedAtAction("PutEducation", new { id = sanitizedEducation.ResumeId }, EducationDTO);
            }
        }

        [HttpPost]
        public async Task<ActionResult<EducationDTO>> PostEducation(Education Education)
        {
            EducationDTO EducationDTO = EducationToDTO(Education);
            Education sanitizedEducation = DTOToEducation(EducationDTO);
            _context.Education.Add(sanitizedEducation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostEducation", new { id = sanitizedEducation.ResumeId }, EducationDTO);
        }

        // DELETE: api/Education/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<EducationDTO>> DeleteEducation(long id)
        {
            var Education = await _context.Education.FindAsync(id);
            if (Education == null)
            {
                return NotFound();
            }

            _context.Education.Remove(Education);
            await _context.SaveChangesAsync();

            return EducationToDTO(Education);
        }

        private bool EducationExists(long id)
        {
            return _context.Education.Any(e => e.ResumeId == id);
        }

        private static EducationDTO EducationToDTO(Education Education) =>
            new EducationDTO
            {
                ResumeId = Education.ResumeId,
                Order = Education.Order,
                Name = Education.Name,
                StartDate = Education.StartDate,
                EndDate = Education.EndDate,
                Major = Education.Major
            };

        private static Education DTOToEducation(EducationDTO EducationDTO) =>
            new Education
            {
                ResumeId = EducationDTO.ResumeId,
                Order = EducationDTO.Order,
                Name = EducationDTO.Name,
                StartDate = EducationDTO.StartDate,
                EndDate = EducationDTO.EndDate,
                Major = EducationDTO.Major,
                Resume = null
            };
    }
}
