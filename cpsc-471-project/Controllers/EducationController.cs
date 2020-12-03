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
    [Route("api/resumes")]
    [ApiController]
    public class EducationsController : ControllerBase
    {
        private readonly JobHunterDBContext _context;

        public EducationsController(JobHunterDBContext context)
        {
            _context = context;
        }

        [HttpPut("{id}/Educations/{order}")]
        public async Task<IActionResult> PutEducation(long id, long order, EducationDTO educationDTO)
        {
            Education sanitizedEducation = DTOToEducation(educationDTO);
            if (id != sanitizedEducation.ResumeId)
            {
                return BadRequest();
            }

            if (order != sanitizedEducation.Order)
            {
                return BadRequest();
            }

            bool newEducation;
            if (!EducationExists(id, order))
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
                if (!EducationExists(id, order))
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
                return CreatedAtAction("PutEducation", new { id = sanitizedEducation.ResumeId, order = sanitizedEducation.Order }, educationDTO);
            }
            else
            {
                return AcceptedAtAction("PutEducation", new { id = sanitizedEducation.ResumeId, order = sanitizedEducation.Order }, educationDTO);
            }
        }

        [HttpPost("{id}/Educations")]
        public async Task<ActionResult<EducationDTO>> PostEducation(EducationDTO educationDTO)
        {
            Education sanitizedEducation = DTOToEducation(educationDTO);
            _context.Education.Add(sanitizedEducation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostEducation", new { id = sanitizedEducation.ResumeId, order = sanitizedEducation.Order }, educationDTO);
        }

        // DELETE: api/Education/5
        [HttpDelete("{id}/Educations/{order}")]
        public async Task<ActionResult<EducationDTO>> DeleteEducation(long id, long order)
        {
            var education = await _context.Education.FindAsync(id, order);
            if (education == null)
            {
                return NotFound();
            }

            _context.Education.Remove(education);
            await _context.SaveChangesAsync();

            return EducationToDTO(education);
        }

        private bool EducationExists(long id, long order)
        {
            return _context.Education.Any(e => (e.ResumeId == id) && (e.Order == order));
        }

        private static EducationDTO EducationToDTO(Education education) =>
            new EducationDTO
            {
                ResumeId = education.ResumeId,
                Order = education.Order,
                Name = education.Name,
                StartDate = education.StartDate,
                EndDate = education.EndDate,
                Major = education.Major
            };

        private static Education DTOToEducation(EducationDTO educationDTO) =>
            new Education
            {
                ResumeId = educationDTO.ResumeId,
                Order = educationDTO.Order,
                Name = educationDTO.Name,
                StartDate = educationDTO.StartDate,
                EndDate = educationDTO.EndDate,
                Major = educationDTO.Major,
                Resume = null
            };
    }
}
