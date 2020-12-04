using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore;

using cpsc_471_project.Models;

namespace cpsc_471_project.Controllers
{
    [Route("api")]
    [ApiController]
    public class ReferralsController : ControllerBase
    {
        UserManager<User> userManager;
        private readonly JobHunterDBContext _context;

        public ReferralsController(JobHunterDBContext context, UserManager<User> userManager)
        {
            this.userManager = userManager;
            _context = context;
        }

        // GET: api/Referrals
        [HttpGet("referrals")]
        public async Task<ActionResult<IEnumerable<ReferralDTO>>> GetReferrals()
        {
            var app = await _context.Referrals.ToListAsync();

            // NOTE: the select function here is not querying anything
            // it is simply converting the values to another format
            // i.e. the functional programming map function is named Select in C#
            return app.Select(x => ReferralToDTO(x)).ToList();
        }

        [HttpGet("applications/{appId}/referrals/{name}")]
        public async Task<ActionResult<ReferralDTO>> GetReferral(long appId, long refId)
        {
            var referral = await _context.Referrals.FindAsync(appId, refId);

            if (referral == null)
            {
                return NotFound();
            }

            return ReferralToDTO(referral);
        }

        [HttpPatch("applications/{appId}/referrals/{name}")]
        public async Task<IActionResult> PatchReferral(long appId, long refId, ReferralDTO referralDTO)
        {
            Referral referral = DTOToReferral(referralDTO);
            if (appId != referralDTO.ApplicationId || refId != referralDTO.ReferralId)
            {
                return BadRequest();
            }

            _context.Entry(referral).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReferralExists(appId, refId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return AcceptedAtAction("PatchOffer", new { ApplicationId = referral.ApplicationId, ReferralId = referral.ReferralId }, referralDTO);
        }

        [HttpPost("applications/{appId}/referrals")] // should this potentially just be "Referrals"?
        public async Task<ActionResult<ReferralDTO>> PostReferral(ReferralDTO referralDTO)
        {
            Referral referral = DTOToReferral(referralDTO);
            _context.Referrals.Add(referral);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostReferral", new { AppliationId = referral.ApplicationId, ReferralId = referral.ReferralId }, referralDTO);

        }

        // DELETE: api/Skill/5
        [HttpDelete("applications/{appId}/referrals/{name}")]
        public async Task<ActionResult<ReferralDTO>> DeleteReferral(long appId, long refId)
        {
            var referral = await _context.Referrals.FindAsync(appId, refId);
            if (referral == null)
            {
                return NotFound();
            }

            _context.Referrals.Remove(referral);
            await _context.SaveChangesAsync();

            return ReferralToDTO(referral);
        }

        private bool ReferralExists(long appId, long refId)
        {
            return _context.Referrals.Any(e => (e.ReferralId == refId && e.ApplicationId == appId));
        }

        private static ReferralDTO ReferralToDTO(Referral r) =>
            new ReferralDTO
            {
                ApplicationId = r.ApplicationId,
                ReferralId = r.ReferralId,
                Name = r.Name,
                Email = r.Email,
                Position = r.Position,
                Company = r.Company,
                Phone = r.Phone
            };

        private static Referral DTOToReferral(ReferralDTO rDTO) =>
            new Referral
            {
                ApplicationId = rDTO.ApplicationId,
                Application = null,
                ReferralId = rDTO.ReferralId,
                Name = rDTO.Name,
                Email = rDTO.Email,
                Position = rDTO.Position,
                Company = rDTO.Company,
                Phone = rDTO.Phone
            };
    }
}
