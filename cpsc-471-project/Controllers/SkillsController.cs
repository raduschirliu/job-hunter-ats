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
    public class SkillsController : ControllerBase
    {
        private readonly JobHunterDBContext _context;

        public SkillsController(JobHunterDBContext context)
        {
            _context = context;
        }

        // GET: api/Skill
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SkillDTO>>> GetSkill()
        {
            var Skill = await _context.Skills.ToListAsync();

            // NOTE: the select function here is not querying anything
            // it is simply converting the values to another format
            // i.e. the functional programming map function is named Select in C#
            return Skill.Select(x => SkillToDTO(x)).ToList();
        }

        // GET: api/Skill/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SkillDTO>> GetSkill(long id)
        {
            var Skill = await _context.Skills.FindAsync(id);

            if (Skill == null)
            {
                return NotFound();
            }

            return SkillToDTO(Skill);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSkill(long id, Skill Skill)
        {
            SkillDTO SkillDTO = SkillToDTO(Skill);
            Skill sanitizedSkill = DTOToSkill(SkillDTO);
            if (id != sanitizedSkill.ResumeId)
            {
                return BadRequest();
            }

            bool newSkill;
            if (!SkillExists(id))
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
                if (!SkillExists(id))
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
                return CreatedAtAction("PutSkill", new { id = sanitizedSkill.ResumeId }, SkillDTO);
            }
            else
            {
                return AcceptedAtAction("PutSkill", new { id = sanitizedSkill.ResumeId }, SkillDTO);
            }
        }

        [HttpPost]
        public async Task<ActionResult<SkillDTO>> PostSkill(Skill Skill)
        {
            SkillDTO SkillDTO = SkillToDTO(Skill);
            Skill sanitizedSkill = DTOToSkill(SkillDTO);
            _context.Skills.Add(sanitizedSkill);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostSkill", new { id = sanitizedSkill.ResumeId }, SkillDTO);
        }

        // DELETE: api/Skill/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SkillDTO>> DeleteSkill(long id)
        {
            var Skill = await _context.Skills.FindAsync(id);
            if (Skill == null)
            {
                return NotFound();
            }

            _context.Skills.Remove(Skill);
            await _context.SaveChangesAsync();

            return SkillToDTO(Skill);
        }

        private bool SkillExists(long id)
        {
            return _context.Skills.Any(e => e.ResumeId == id);
        }

        private static SkillDTO SkillToDTO(Skill Skill) =>
            new SkillDTO
            {
                ResumeId = Skill.ResumeId,
                Order = Skill.Order,
                Name = Skill.Name,
                Proficiency = Skill.Proficiency
            };

        private static Skill DTOToSkill(SkillDTO SkillDTO) =>
            new Skill
            {
                ResumeId = SkillDTO.ResumeId,
                Order = SkillDTO.Order,
                Name = SkillDTO.Name,
                Proficiency = SkillDTO.Proficiency,
                Resume = null
            };
    }
}
