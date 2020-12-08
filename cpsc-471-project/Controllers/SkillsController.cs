using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using cpsc_471_project.Authentication;
using cpsc_471_project.Models;


namespace cpsc_471_project.Controllers
{
    [Route("api/resumes")]
    [ApiController]
    public class SkillsController : ResumeSectionController
    {
        UserManager<User> userManager;
        private readonly JobHunterDBContext _context;

        public SkillsController(JobHunterDBContext context, UserManager<User> userManager) : base(context, userManager) { }

        [Authorize]
        [HttpPatch("{resumeId}/skills/{order}")]
        public async Task<IActionResult> PatchSkill(long resumeId, long order, SkillDTO skillDTO)
        {
            Skill sanitizedSkill = DTOToSkill(skillDTO);
            if (resumeId != sanitizedSkill.ResumeId)
            {
                return BadRequest("resumeId in query params does not match resumeId in body");
            }

            if (order != sanitizedSkill.Order)
            {
                return BadRequest("subsection order in query params does not match subsection order in body");
            }

            if (!await ResumeAccessAuthorized(sanitizedSkill.ResumeId))
            {
                return GenerateResumeNotFoundError(sanitizedSkill.ResumeId);
            }

            if (!SkillExists(resumeId, order))
            {
                return BadRequest("Associated subsection does not exist");
            }
            _context.Entry(sanitizedSkill).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return AcceptedAtAction("PatchSkill", new { resumeId = sanitizedSkill.ResumeId, order = sanitizedSkill.Order }, skillDTO);
        }

        [HttpPost("{resumeId}/Skills")]
        public async Task<ActionResult<SkillDTO>> PostSkill(SkillDTO skillDTO)
        {
            Skill sanitizedSkill = DTOToSkill(skillDTO);

            if (!await ResumeAccessAuthorized(sanitizedSkill.ResumeId))
            {
                return GenerateResumeNotFoundError(sanitizedSkill.ResumeId);
            }

            _context.Skills.Add(sanitizedSkill);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostSkill", new { resumeId = sanitizedSkill.ResumeId, order = sanitizedSkill.Order }, skillDTO);
        }

        // DELETE: api/Skill/5
        [HttpDelete("{resumeId}/Skills/{order}")]
        public async Task<ActionResult<SkillDTO>> DeleteSkill(long resumeId, long order)
        {
            if (!await ResumeAccessAuthorized(resumeId))
            {
                return GenerateResumeNotFoundError(resumeId);
            }

            var skill = await _context.Skills.FindAsync(resumeId, order);
            if (skill == null)
            {
                return NotFound();
            }

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();

            return SkillToDTO(skill);
        }

        private bool SkillExists(long resumeId, long order)
        {
            return _context.Skills.Any(e => (e.ResumeId == resumeId) && (e.Order == order));
        }

        public static SkillDTO SkillToDTO(Skill skill) =>
            new SkillDTO
            {
                ResumeId = skill.ResumeId,
                Order = skill.Order,
                Name = skill.Name,
                Proficiency = skill.Proficiency
            };

        public static Skill DTOToSkill(SkillDTO skillDTO) =>
            new Skill
            {
                ResumeId = skillDTO.ResumeId,
                Order = skillDTO.Order,
                Name = skillDTO.Name,
                Proficiency = skillDTO.Proficiency,
                Resume = null
            };
    }
}
