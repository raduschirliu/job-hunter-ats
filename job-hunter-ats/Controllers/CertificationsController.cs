using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using job_hunter_ats.Authentication;
using job_hunter_ats.Models;

namespace job_hunter_ats.Controllers
{
    [Route("api/resumes")]
    [ApiController]
    public class CertificationsController : ResumeSectionController
    {
        public CertificationsController(JobHunterDBContext context, UserManager<User> userManager): base(context, userManager) {}

        [Authorize]
        [HttpPatch("{resumeId}/certifications/{order}")]
        public async Task<IActionResult> PatchCertification(long resumeId, long order, CertificationDTO certificationDTO)
        {
            Certification sanitizedCertification = DTOToCertification(certificationDTO);
            if (resumeId != sanitizedCertification.ResumeId)
            {
                return BadRequest("resumeId in query params does not match resumeId in body");
            }

            if (order != sanitizedCertification.Order)
            {
                return BadRequest("subsection order in query params does not match subsection order in body");
            }

            if (!await ResumeAccessAuthorized(sanitizedCertification.ResumeId))
            {
                return GenerateResumeNotFoundError(sanitizedCertification.ResumeId);
            }

            if (!CertificationExists(resumeId, order))
            {
                return BadRequest("Associated subsection does not exist");
            }
            _context.Entry(sanitizedCertification).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return AcceptedAtAction("PatchCertification", new { resumeId = sanitizedCertification.ResumeId, order = sanitizedCertification.Order }, certificationDTO);
        }

        [HttpPost("{resumeId}/certifications")]
        public async Task<ActionResult<CertificationDTO>> PostCertification(CertificationDTO certificationDTO)
        {
            Certification sanitizedCertification = DTOToCertification(certificationDTO);

            if (!await ResumeAccessAuthorized(sanitizedCertification.ResumeId))
            {
                return GenerateResumeNotFoundError(sanitizedCertification.ResumeId);
            }

            if (CertificationExists(sanitizedCertification.ResumeId, certificationDTO.Order))
            {
                return BadRequest("associated subsection already exists");
            }
            _context.Certifications.Add(sanitizedCertification);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostCertification", new { resumeId = sanitizedCertification.ResumeId, order = sanitizedCertification.Order }, certificationDTO);
        }

        // DELETE: api/Certification/5
        [HttpDelete("{resumeId}/certifications/{order}")]
        public async Task<ActionResult<CertificationDTO>> DeleteCertification(long resumeId, long order)
        {
            if (!await ResumeAccessAuthorized(resumeId))
            {
                return GenerateResumeNotFoundError(resumeId);
            }

            var certification = await _context.Certifications.FindAsync(resumeId, order);
            if (certification == null)
            {
                return NotFound("Subsection not found");
            }

            _context.Certifications.Remove(certification);
            await _context.SaveChangesAsync();

            return CertificationToDTO(certification);
        }

        private bool CertificationExists(long resumeId, long order)
        {
            return _context.Certifications.Any(e => (e.ResumeId == resumeId) && (e.Order == order));
        }

        public static CertificationDTO CertificationToDTO(Certification certification) =>
            new CertificationDTO
            {
                ResumeId = certification.ResumeId,
                Order = certification.Order,
                Name = certification.Name,
                Source = certification.Source
            };

        public static Certification DTOToCertification(CertificationDTO certificationDTO) =>
            new Certification
            {
                ResumeId = certificationDTO.ResumeId,
                Order = certificationDTO.Order,
                Name = certificationDTO.Name,
                Source = certificationDTO.Source,
                Resume = null
            };
    }
}
