using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using job_hunter_ats.Authentication;
using job_hunter_ats.Models;


namespace job_hunter_ats.Controllers
{
    [Route("api/resumes")]
    [ApiController]
    public class EducationController : ResumeSectionController
    {
        public EducationController(JobHunterDBContext context, UserManager<User> userManager) : base(context, userManager) { }

        [Authorize]
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

            if (!await ResumeAccessAuthorized(sanitizedEducation.ResumeId))
            {
                return GenerateResumeNotFoundError(sanitizedEducation.ResumeId);
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

        [Authorize]
        [HttpPost("{resumeId}/education")]
        public async Task<ActionResult<EducationDTO>> PostEducation(EducationDTO educationDTO)
        {
            Education sanitizedEducation = DTOToEducation(educationDTO);

            if (!await ResumeAccessAuthorized(sanitizedEducation.ResumeId))
            {
                return GenerateResumeNotFoundError(sanitizedEducation.ResumeId);
            }

            _context.Education.Add(sanitizedEducation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostEducation", new { resumeId = sanitizedEducation.ResumeId, order = sanitizedEducation.Order }, educationDTO);
        }

        // DELETE: api/education/5
        [Authorize]
        [HttpDelete("{resumeId}/education/{order}")]
        public async Task<ActionResult<EducationDTO>> DeleteEducation(long resumeId, long order)
        {
            if (!await ResumeAccessAuthorized(resumeId))
            {
                return GenerateResumeNotFoundError(resumeId);
            }

            var education = await _context.Education.FindAsync(resumeId, order);
            if (education == null)
            {
                return NotFound("Subsection not found");
            }

            _context.Education.Remove(education);
            await _context.SaveChangesAsync();

            return EducationToDTO(education);
        }

        private bool EducationExists(long resumeId, long order)
        {
            return _context.Education.Any(e => (e.ResumeId == resumeId) && (e.Order == order));
        }

        public static EducationDTO EducationToDTO(Education education)
        {
            DateTime? inputStartDate = education.StartDate;
            if (education.StartDate.HasValue)
            {
                inputStartDate = education.StartDate.GetValueOrDefault();
            }
            DateTime? inputEndDate = education.EndDate;
            if (education.EndDate.HasValue)
            {
                inputEndDate = education.EndDate.GetValueOrDefault();
            }
            return new EducationDTO
            {
                ResumeId = education.ResumeId,
                Order = education.Order,
                SchoolName = education.SchoolName,
                StartDate = inputStartDate,
                EndDate = inputEndDate,
                Major = education.Major
            };
        }

        public static Education DTOToEducation(EducationDTO educationDTO)
        {
            DateTime? inputStartDate = educationDTO.StartDate;
            if (educationDTO.StartDate.HasValue)
            {
                inputStartDate = educationDTO.StartDate.GetValueOrDefault();
            }
            DateTime? inputEndDate = educationDTO.EndDate;
            if (educationDTO.EndDate.HasValue)
            {
                inputEndDate = educationDTO.EndDate.GetValueOrDefault();
            }
            return new Education
            {
                ResumeId = educationDTO.ResumeId,
                Order = educationDTO.Order,
                SchoolName = educationDTO.SchoolName,
                StartDate = inputStartDate,
                EndDate = inputEndDate,
                Major = educationDTO.Major,
                Resume = null
            };
        }  
    }
}
