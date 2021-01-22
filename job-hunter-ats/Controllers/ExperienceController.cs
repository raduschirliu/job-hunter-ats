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
    public class ExperienceController : ResumeSectionController
    {
        public ExperienceController(JobHunterDBContext context, UserManager<User> userManager) : base(context, userManager) { }

        [Authorize]
        [HttpPatch("{resumeId}/experience/{order}")]
        public async Task<IActionResult> PatchExperience(long resumeId, long order, ExperienceDTO experienceDTO)
        {
            Experience sanitizedExperience = DTOToExperience(experienceDTO);
            if (resumeId != sanitizedExperience.ResumeId)
            {
                return BadRequest("resumeId in query params does not match resumeId in body");
            }

            if (order != sanitizedExperience.Order)
            {
                return BadRequest("subsection order in query params does not match subsection order in body");
            }

            if (!await ResumeAccessAuthorized(sanitizedExperience.ResumeId))
            {
                return GenerateResumeNotFoundError(sanitizedExperience.ResumeId);
            }

            if (!ExperienceExists(resumeId, order))
            {
                return BadRequest("Associated subsection does not exist");
            }
            _context.Entry(sanitizedExperience).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return AcceptedAtAction("PatchExperience", new { resumeId = sanitizedExperience.ResumeId, order = sanitizedExperience.Order }, experienceDTO);
        }

        [Authorize]
        [HttpPost("{resumeId}/experience")]
        public async Task<ActionResult<ExperienceDTO>> PostExperience(ExperienceDTO experienceDTO)
        {
            Experience sanitizedExperience = DTOToExperience(experienceDTO);

            if (!await ResumeAccessAuthorized(sanitizedExperience.ResumeId))
            {
                return GenerateResumeNotFoundError(sanitizedExperience.ResumeId);
            }


            _context.Experiences.Add(sanitizedExperience);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostExperience", new { resumeId = sanitizedExperience.ResumeId, order = sanitizedExperience.Order }, experienceDTO);
        }

        // DELETE: api/experience/5
        [Authorize]
        [HttpDelete("{resumeId}/experience/{order}")]
        public async Task<ActionResult<ExperienceDTO>> DeleteExperience(long resumeId, long order)
        {
            if (!await ResumeAccessAuthorized(resumeId))
            {
                return GenerateResumeNotFoundError(resumeId);
            }

            var experience = await _context.Experiences.FindAsync(resumeId, order);
            if (experience == null)
            {
                return NotFound("Subsection not found");
            }

            _context.Experiences.Remove(experience);
            await _context.SaveChangesAsync();

            return ExperienceToDTO(experience);
        }

        private bool ExperienceExists(long resumeId, long order)
        {
            return _context.Experiences.Any(e => (e.ResumeId == resumeId) && (e.Order == order));
        }

        public static ExperienceDTO ExperienceToDTO(Experience experience)
        {
            DateTime inputStartDate = experience.StartDate.Date;
            DateTime? inputEndDate = experience.EndDate;
            if (experience.EndDate.HasValue)
            {
                inputEndDate = experience.EndDate.GetValueOrDefault();
            }
            return new ExperienceDTO
            {
                ResumeId = experience.ResumeId,
                Order = experience.Order,
                Title = experience.Title,
                Company = experience.Company,
                StartDate = experience.StartDate,
                EndDate = experience.EndDate
            };
        }

        public static Experience DTOToExperience(ExperienceDTO experienceDTO)
        {
            DateTime inputStartDate = experienceDTO.StartDate.Date;
            DateTime? inputEndDate = experienceDTO.EndDate;
            if (experienceDTO.EndDate.HasValue)
            {
                inputEndDate = experienceDTO.EndDate.GetValueOrDefault();
            }
            return new Experience
            {
                ResumeId = experienceDTO.ResumeId,
                Order = experienceDTO.Order,
                Title = experienceDTO.Title,
                Company = experienceDTO.Company,
                StartDate = inputStartDate,
                EndDate = inputEndDate,
                Resume = null
            };
        }
    }
}
