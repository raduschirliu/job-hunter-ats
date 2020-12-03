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

        [HttpPut("{id}/awards/{order}")]
        public async Task<IActionResult> PutAward(long id, long order, AwardDTO awardDTO)
        {
            Award sanitizedAward = DTOToAward(awardDTO);
            if (id != sanitizedAward.ResumeId)
            {
                return BadRequest();
            }

            if (order != sanitizedAward.Order)
            {
                return BadRequest();
            }

            bool newAward;
            if (!AwardExists(id, order))
            {
                newAward = true;
                _context.Awards.Add(sanitizedAward);
            }
            else
            {
                newAward = false;
                _context.Entry(sanitizedAward).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AwardExists(id, order))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            if (newAward)
            {
                return CreatedAtAction("PutAward", new { id = sanitizedAward.ResumeId, order = sanitizedAward.Order }, awardDTO);
            }
            else
            {
                return AcceptedAtAction("PutAward", new { id = sanitizedAward.ResumeId, order = sanitizedAward.Order }, awardDTO);
            }
        }

        [HttpPost("{id}/awards")]
        public async Task<ActionResult<AwardDTO>> PostAward(AwardDTO awardDTO)
        {
            Award sanitizedAward = DTOToAward(awardDTO);
            _context.Awards.Add(sanitizedAward);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostAward", new { id = sanitizedAward.ResumeId, order = sanitizedAward.Order }, awardDTO);
        }

        // DELETE: api/Award/5
        [HttpDelete("{id}/awards/{order}")]
        public async Task<ActionResult<AwardDTO>> DeleteAward(long id, long order)
        {
            var award = await _context.Awards.FindAsync(id, order);
            if (award == null)
            {
                return NotFound();
            }

            _context.Awards.Remove(award);
            await _context.SaveChangesAsync();

            return AwardToDTO(award);
        }

        private bool AwardExists(long id, long order)
        {
            return _context.Awards.Any(e => (e.ResumeId == id) && (e.Order == order));
        }

        private static AwardDTO AwardToDTO(Award award) =>
            new AwardDTO
            {
                ResumeId = award.ResumeId,
                Order = award.Order,
                Name = award.Name,
                DateReceived = award.DateReceived,
                value = award.value
            };

        private static Award DTOToAward(AwardDTO awardDTO) =>
            new Award
            {
                ResumeId = awardDTO.ResumeId,
                Order = awardDTO.Order,
                Name = awardDTO.Name,
                DateReceived = awardDTO.DateReceived,
                value = awardDTO.value,
                Resume = null
            };
    }
}
