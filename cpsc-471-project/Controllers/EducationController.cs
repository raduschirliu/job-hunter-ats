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

        [HttpPatch("{resumeId}/education/{order}")]
        public async Task<IActionResult> PatchEducation(long resumeId, long order, EducationDTO educationDTO)
        {
            Education sanitizedEducation = DTOToEducation(educationDTO);
            if (resumeId != sanitizedEducation.ResumeId)
            {
                return BadRequest("resumeId in query params does not match resumeId in body");
            }

            if (order != sanitizedEducation.Order)
            {
                return BadRequest("subsection order in query params does not match subsection order in body");
            }

            if (!EducationExists(resumeId, order))
            {
                return BadRequest("Associated subsection does not exist");
            }
            _context.Entry(sanitizedEducation).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return AcceptedAtAction("PatchEducation", new { resumeId = sanitizedEducation.ResumeId, order = sanitizedEducation.Order }, educationDTO);
        }

        [HttpPost("{resumeId}/education")]
        public async Task<ActionResult<EducationDTO>> PostEducation(EducationDTO educationDTO)
        {
            Education sanitizedEducation = DTOToEducation(educationDTO);
            _context.Education.Add(sanitizedEducation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostEducation", new { resumeId = sanitizedEducation.ResumeId, order = sanitizedEducation.Order }, educationDTO);
        }

        // DELETE: api/Education/5
        [HttpDelete("{resumeId}/educations/{order}")]
        public async Task<ActionResult<EducationDTO>> DeleteEducation(long resumeId, long order)
        {
            var education = await _context.Education.FindAsync(resumeId, order);
            if (education == null)
            {
                return NotFound();
            }

            _context.Education.Remove(education);
            await _context.SaveChangesAsync();

            return EducationToDTO(education);
        }

        private bool EducationExists(long resumeId, long order)
        {
            return _context.Education.Any(e => (e.ResumeId == resumeId) && (e.Order == order));
        }

        private static EducationDTO EducationToDTO(Education education) =>
            new EducationDTO
            {
                ResumeId = education.ResumeId,
                Order = education.Order,
                SchoolName = education.SchoolName,
                StartDate = education.StartDate,
                EndDate = education.EndDate,
                Major = education.Major
            };

        private static Education DTOToEducation(EducationDTO educationDTO) =>
            new Education
            {
                ResumeId = educationDTO.ResumeId,
                Order = educationDTO.Order,
                SchoolName = educationDTO.SchoolName,
                StartDate = educationDTO.StartDate,
                EndDate = educationDTO.EndDate,
                Major = educationDTO.Major,
                Resume = null
            };
    }
}
