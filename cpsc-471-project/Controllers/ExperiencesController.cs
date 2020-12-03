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
    public class ExperiencesController : ControllerBase
    {
        private readonly JobHunterDBContext _context;

        public ExperiencesController(JobHunterDBContext context)
        {
            _context = context;
        }

        [HttpPut("{id}/Experiences/{order}")]
        public async Task<IActionResult> PutExperience(long id, long order, ExperienceDTO experienceDTO)
        {
            Experience sanitizedExperience = DTOToExperience(experienceDTO);
            if (id != sanitizedExperience.ResumeId)
            {
                return BadRequest();
            }

            if (order != sanitizedExperience.Order)
            {
                return BadRequest();
            }

            bool newExperience;
            if (!ExperienceExists(id, order))
            {
                newExperience = true;
                _context.Experiences.Add(sanitizedExperience);
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
                if (!ExperienceExists(id, order))
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
                return CreatedAtAction("PutExperience", new { id = sanitizedExperience.ResumeId, order = sanitizedExperience.Order }, experienceDTO);
            }
            else
            {
                return AcceptedAtAction("PutExperience", new { id = sanitizedExperience.ResumeId, order = sanitizedExperience.Order }, experienceDTO);
            }
        }

        [HttpPost("{id}/Experiences")]
        public async Task<ActionResult<ExperienceDTO>> PostExperience(ExperienceDTO experienceDTO)
        {
            Experience sanitizedExperience = DTOToExperience(experienceDTO);
            _context.Experiences.Add(sanitizedExperience);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostExperience", new { id = sanitizedExperience.ResumeId, order = sanitizedExperience.Order }, experienceDTO);
        }

        // DELETE: api/Experience/5
        [HttpDelete("{id}/Experiences/{order}")]
        public async Task<ActionResult<ExperienceDTO>> DeleteExperience(long id, long order)
        {
            var experience = await _context.Experiences.FindAsync(id, order);
            if (experience == null)
            {
                return NotFound();
            }

            _context.Experiences.Remove(experience);
            await _context.SaveChangesAsync();

            return ExperienceToDTO(experience);
        }

        private bool ExperienceExists(long id, long order)
        {
            return _context.Experiences.Any(e => (e.ResumeId == id) && (e.Order == order));
        }

        private static ExperienceDTO ExperienceToDTO(Experience experience) =>
            new ExperienceDTO
            {
                ResumeId = experience.ResumeId,
                Order = experience.Order,
                Title = experience.Title,
                Company = experience.Company,
                StartDate = experience.StartDate,
                EndDate = experience.EndDate
            };

        private static Experience DTOToExperience(ExperienceDTO experienceDTO) =>
            new Experience
            {
                ResumeId = experienceDTO.ResumeId,
                Order = experienceDTO.Order,
                Title = experienceDTO.Title,
                Company = experienceDTO.Company,
                StartDate = experienceDTO.StartDate,
                EndDate = experienceDTO.EndDate,
                Resume = null
            };
    }
}
