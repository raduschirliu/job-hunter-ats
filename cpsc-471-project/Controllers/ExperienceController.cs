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
    public class ExperienceController : ControllerBase
    {
        private readonly JobHunterDBContext _context;

        public ExperienceController(JobHunterDBContext context)
        {
            _context = context;
        }

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

        [HttpPost("{resumeId}/experience")]
        public async Task<ActionResult<ExperienceDTO>> PostExperience(ExperienceDTO experienceDTO)
        {
            Experience sanitizedExperience = DTOToExperience(experienceDTO);
            _context.Experiences.Add(sanitizedExperience);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostExperience", new { resumeId = sanitizedExperience.ResumeId, order = sanitizedExperience.Order }, experienceDTO);
        }

        // DELETE: api/Experience/5
        [HttpDelete("{resumeId}/experience/{order}")]
        public async Task<ActionResult<ExperienceDTO>> DeleteExperience(long resumeId, long order)
        {
            var experience = await _context.Experiences.FindAsync(resumeId, order);
            if (experience == null)
            {
                return NotFound();
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
