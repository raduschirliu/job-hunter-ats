using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore;

using cpsc_471_project.Authentication;
using cpsc_471_project.Models;

namespace cpsc_471_project.Controllers
{
    [Route("api")]
    [ApiController]
    public class OffersController : ControllerBase
    {
        UserManager<User> userManager;
        private readonly JobHunterDBContext _context;

        public OffersController(JobHunterDBContext context, UserManager<User> userManager)
        {
            this.userManager = userManager;
            _context = context;
        }

        // GET: api/Offers
        [HttpGet("offers")]
        public async Task<ActionResult<IEnumerable<OfferDTO>>> GetOffers()
        {
            var app = await _context.Offers.ToListAsync();

            // NOTE: the select function here is not querying anything
            // it is simply converting the values to another format
            // i.e. the functional programming map function is named Select in C#
            return app.Select(x => OfferToDTO(x)).ToList();
        }

        // GET: api/Applications/{applicationid}/Offers/{offerid}
        [HttpGet("applications/{appId}/offers/{offerId}")]
        public async Task<ActionResult<OfferDTO>> GetOffer(long appId, long offerId)
        {
            var offer = await _context.Offers.FindAsync(appId, offerId);

            if (offer == null)
            {
                return NotFound();
            }

            return OfferToDTO(offer);
        }

        [Authorize(Roles = UserRoles.Admin + ", " + UserRoles.Recruiter)]
        [HttpPatch("applications/{appId}/offers/{offerId}")]
        public async Task<IActionResult> PatchOffer(long appId, long offerId, OfferDTO offerDTO)
        {
            Offer offer = DTOToOffer(offerDTO);
            if (offerId != offerDTO.OfferId || appId != offerDTO.ApplicationId)
            {
                return BadRequest();
            }

            User user = await userManager.FindByNameAsync(User.Identity.Name);
            IList<string> roles = await userManager.GetRolesAsync(user);

            if (!roles.Contains(UserRoles.Admin))
            {
                var query = from queryOffer in _context.Offers
                            join application in _context.Applications on queryOffer.ApplicationId equals application.ApplicationId
                            join jobPost in _context.JobPosts on application.JobId equals jobPost.JobPostId
                            join recruiter in _context.Recruiters on user.Id equals recruiter.UserId
                            where jobPost.CompanyId == recruiter.CompanyId && offer.ApplicationId == appId && offer.OfferId == offerId
                            select queryOffer;
                if(!await query.AnyAsync())
                {
                    return Unauthorized("Cannot make an offer for that application");
                }
            }

            _context.Entry(offer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OfferExists(appId, offerId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return AcceptedAtAction("PatchOffer", new { ApplicationId = offer.ApplicationId, OfferId = offer.OfferId }, offerDTO);
        }

        // POST: api/applications/1/offers
        [Authorize(Roles = UserRoles.Admin + ", " + UserRoles.Recruiter)]
        [HttpPost("applications/{appId}/offers")]
        public async Task<ActionResult<OfferDTO>> PostOffer(OfferDTO offerDTO)
        {
            Offer offer = DTOToOffer(offerDTO);
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            IList<string> roles = await userManager.GetRolesAsync(user);

            if (!roles.Contains(UserRoles.Admin))
            {
                var query = from application in _context.Applications
                            join jobPost in _context.JobPosts on application.JobId equals jobPost.JobPostId
                            join recruiter in _context.Recruiters on user.Id equals recruiter.UserId
                            where jobPost.CompanyId == recruiter.CompanyId && offer.ApplicationId == application.ApplicationId
                            select application;
                if (!await query.AnyAsync())
                {
                    return Unauthorized("Cannot make an offer for that application");
                }
            }

            _context.Offers.Add(offer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostOffer", new { ApplicationId = offer.ApplicationId, OfferId = offer.OfferId }, offerDTO);

        }

        // DELETE: api/applications/1/offers/2
        [Authorize(Roles = UserRoles.Admin + ", " + UserRoles.Recruiter)]
        [HttpDelete("applications/{appId}/offers/{offerId}")]
        public async Task<ActionResult<OfferDTO>> DeleteOffer(long appId, long offerId)
        {
            var offer = await _context.Offers.FindAsync(appId, offerId);
            if (offer == null)
            {
                return NotFound();
            }
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            IList<string> roles = await userManager.GetRolesAsync(user);

            if (!roles.Contains(UserRoles.Admin))
            {
                var query = from queryOffer in _context.Offers
                            join application in _context.Applications on queryOffer.ApplicationId equals application.ApplicationId
                            join jobPost in _context.JobPosts on application.JobId equals jobPost.JobPostId
                            join recruiter in _context.Recruiters on user.Id equals recruiter.UserId
                            where jobPost.CompanyId == recruiter.CompanyId && offer.ApplicationId == appId && offer.OfferId == offerId
                            select queryOffer;
                if (!await query.AnyAsync())
                {
                    return Unauthorized("Cannot delete that offer");
                }
            }

            _context.Offers.Remove(offer);
            await _context.SaveChangesAsync();

            return OfferToDTO(offer);
        }

        private bool OfferExists(long appId, long offerId)
        {
            return _context.Offers.Any(e => (e.OfferId == offerId && e.ApplicationId == appId));
        }

        private static OfferDTO OfferToDTO(Offer o) =>
            new OfferDTO
            {
                OfferId = o.OfferId,
                ApplicationId = o.ApplicationId,
                AcceptanceEndDate = o.AcceptanceEndDate,
                Text = o.Text
            };

        private static Offer DTOToOffer(OfferDTO oDTO) =>
            new Offer
            {
                OfferId = oDTO.OfferId,
                ApplicationId = oDTO.ApplicationId,
                Application = null,
                AcceptanceEndDate = oDTO.AcceptanceEndDate,
                Text = oDTO.Text
            };
    }
}
