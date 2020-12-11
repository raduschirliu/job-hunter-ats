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
    public class AwardsController : ResumeSectionController
    {
        public AwardsController(JobHunterDBContext context, UserManager<User> userManager): base(context, userManager) {}

        [Authorize]
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

            User user = await userManager.FindByNameAsync(User.Identity.Name);
            IList<string> roles = await userManager.GetRolesAsync(user);

            if (!await ResumeAccessAuthorized(sanitizedAward.ResumeId))
            {
                return GenerateResumeNotFoundError(sanitizedAward.ResumeId);
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

        [Authorize]
        [HttpPost("{resumeId}/awards")]
        public async Task<ActionResult<AwardDTO>> PostAward(AwardDTO awardDTO)
        {
            Award sanitizedAward = DTOToAward(awardDTO);

            if (!await ResumeAccessAuthorized(sanitizedAward.ResumeId))
            {
                return GenerateResumeNotFoundError(sanitizedAward.ResumeId);
            }

            if (AwardExists(sanitizedAward.ResumeId, sanitizedAward.Order))
            {
                return BadRequest("associated subsection already exists");
            }

            _context.Awards.Add(sanitizedAward);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostAward", new { resumeId = sanitizedAward.ResumeId, order = sanitizedAward.Order }, awardDTO);
        }

        // DELETE: api/awards/5
        [Authorize]
        [HttpDelete("{resumeId}/awards/{order}")]
        public async Task<ActionResult<AwardDTO>> DeleteAward(long resumeId, long order)
        {
            if (!await ResumeAccessAuthorized(resumeId))
            {
                return GenerateResumeNotFoundError(resumeId);
            }

            var award = await _context.Awards.FindAsync(resumeId, order);
            if (award == null)
            {
                return NotFound("Subsection not found");
            }

            _context.Awards.Remove(award);
            await _context.SaveChangesAsync();

            return AwardToDTO(award);
        }

        private bool AwardExists(long resumeId, long order)
        {
            return _context.Awards.Any(e => (e.ResumeId == resumeId) && (e.Order == order));
        }

        public static AwardDTO AwardToDTO(Award award)
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
                value = award.Value
            };
        }

        public static Award DTOToAward(AwardDTO awardDTO)
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
                Value = awardDTO.value,
                Resume = null
            };
        }
    }
}
