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
    public class SkillsController : ControllerBase
    {
        private readonly JobHunterDBContext _context;

        public SkillsController(JobHunterDBContext context)
        {
            _context = context;
        }

        [HttpPut("{id}/Skills/{order}")]
        public async Task<IActionResult> PutSkill(long id, long order, SkillDTO skillDTO)
        {
            Skill sanitizedSkill = DTOToSkill(skillDTO);
            if (id != sanitizedSkill.ResumeId)
            {
                return BadRequest();
            }

            if (order != sanitizedSkill.Order)
            {
                return BadRequest();
            }

            bool newSkill;
            if (!SkillExists(id, order))
            {
                newSkill = true;
                _context.Skills.Add(sanitizedSkill);
            }
            else
            {
                newSkill = false;
                _context.Entry(sanitizedSkill).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SkillExists(id, order))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            if (newSkill)
            {
                return CreatedAtAction("PutSkill", new { id = sanitizedSkill.ResumeId, order = sanitizedSkill.Order }, skillDTO);
            }
            else
            {
                return AcceptedAtAction("PutSkill", new { id = sanitizedSkill.ResumeId, order = sanitizedSkill.Order }, skillDTO);
            }
        }

        [HttpPost("{id}/Skills")]
        public async Task<ActionResult<SkillDTO>> PostSkill(SkillDTO skillDTO)
        {
            Skill sanitizedSkill = DTOToSkill(skillDTO);
            _context.Skills.Add(sanitizedSkill);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostSkill", new { id = sanitizedSkill.ResumeId, order = sanitizedSkill.Order }, skillDTO);
        }

        // DELETE: api/Skill/5
        [HttpDelete("{id}/Skills/{order}")]
        public async Task<ActionResult<SkillDTO>> DeleteSkill(long id, long order)
        {
            var skill = await _context.Skills.FindAsync(id, order);
            if (skill == null)
            {
                return NotFound();
            }

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();

            return SkillToDTO(skill);
        }

        private bool SkillExists(long id, long order)
        {
            return _context.Skills.Any(e => (e.ResumeId == id) && (e.Order == order));
        }

        private static SkillDTO SkillToDTO(Skill skill) =>
            new SkillDTO
            {
                ResumeId = skill.ResumeId,
                Order = skill.Order,
                Name = skill.Name,
                Proficiency = skill.Proficiency
            };

        private static Skill DTOToSkill(SkillDTO skillDTO) =>
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
