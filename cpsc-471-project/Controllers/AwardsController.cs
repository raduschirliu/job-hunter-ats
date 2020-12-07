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
    public class AwardsController : ControllerBase
    {
        private readonly JobHunterDBContext _context;

        public AwardsController(JobHunterDBContext context)
        {
            _context = context;
        }

        [HttpPatch("{resumeId}/awards/{order}")]
        public async Task<IActionResult> PatchAward(long resumeId, long order, AwardDTO awardDTO)
        {
            Award sanitizedAward = DTOToAward(awardDTO);
            if (resumeId != sanitizedAward.ResumeId)
            {
                return BadRequest("resumeId in query params does not match resumeId in body");
            }

            if (order != sanitizedAward.Order)
            {
                return BadRequest("subsection order in query params does not match subsection order in body");
            }

            if (!AwardExists(resumeId, order))
            {
                return BadRequest("Associated subsection does not exist");
            }
            _context.Entry(sanitizedAward).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return AcceptedAtAction("PatchAward", new { resumeId = sanitizedAward.ResumeId, order = sanitizedAward.Order }, awardDTO);
        }

        [HttpPost("{resumeId}/awards")]
        public async Task<ActionResult<AwardDTO>> PostAward(AwardDTO awardDTO)
        {
            Award sanitizedAward = DTOToAward(awardDTO);
            _context.Awards.Add(sanitizedAward);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostAward", new { resumeId = sanitizedAward.ResumeId, order = sanitizedAward.Order }, awardDTO);
        }

        // DELETE: api/Award/5
        [HttpDelete("{resumeId}/awards/{order}")]
        public async Task<ActionResult<AwardDTO>> DeleteAward(long resumeId, long order)
        {
            var award = await _context.Awards.FindAsync(resumeId, order);
            if (award == null)
            {
                return NotFound();
            }

            _context.Awards.Remove(award);
            await _context.SaveChangesAsync();

            return AwardToDTO(award);
        }

        private bool AwardExists(long resumeId, long order)
        {
            return _context.Awards.Any(e => (e.ResumeId == resumeId) && (e.Order == order));
        }

        private static AwardDTO AwardToDTO(Award award)
        {
            DateTime? inputDate = award.DateReceived;
            if (award.DateReceived.HasValue)
            {
                inputDate = award.DateReceived.GetValueOrDefault();
            }
            return new AwardDTO
            {
                ResumeId = award.ResumeId,
                Order = award.Order,
                Name = award.Name,
                DateReceived = inputDate,
                value = award.value
            };
        }

        private static Award DTOToAward(AwardDTO awardDTO)
        {
            DateTime? inputDate = awardDTO.DateReceived;
            if (awardDTO.DateReceived.HasValue)
            {
                inputDate = awardDTO.DateReceived.GetValueOrDefault();
            }
            return new Award
            {
                ResumeId = awardDTO.ResumeId,
                Order = awardDTO.Order,
                Name = awardDTO.Name,
                DateReceived = inputDate,
                value = awardDTO.value,
                Resume = null
            };
        }
    }
}
