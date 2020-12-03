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
    public class CertificationsController : ControllerBase
    {
        private readonly JobHunterDBContext _context;

        public CertificationsController(JobHunterDBContext context)
        {
            _context = context;
        }

        [HttpPut("{id}/Certifications/{order}")]
        public async Task<IActionResult> PutCertification(long id, long order, CertificationDTO certificationDTO)
        {
            Certification sanitizedCertification = DTOToCertification(certificationDTO);
            if (id != sanitizedCertification.ResumeId)
            {
                return BadRequest();
            }

            if (order != sanitizedCertification.Order)
            {
                return BadRequest();
            }

            bool newCertification;
            if (!CertificationExists(id, order))
            {
                newCertification = true;
                _context.Certifications.Add(sanitizedCertification);
            }
            else
            {
                newCertification = false;
                _context.Entry(sanitizedCertification).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CertificationExists(id, order))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            if (newCertification)
            {
                return CreatedAtAction("PutCertification", new { id = sanitizedCertification.ResumeId, order = sanitizedCertification.Order }, certificationDTO);
            }
            else
            {
                return AcceptedAtAction("PutCertification", new { id = sanitizedCertification.ResumeId, order = sanitizedCertification.Order }, certificationDTO);
            }
        }

        [HttpPost("{id}/Certifications")]
        public async Task<ActionResult<CertificationDTO>> PostCertification(CertificationDTO certificationDTO)
        {
            Certification sanitizedCertification = DTOToCertification(certificationDTO);
            _context.Certifications.Add(sanitizedCertification);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostCertification", new { id = sanitizedCertification.ResumeId, order = sanitizedCertification.Order }, certificationDTO);
        }

        // DELETE: api/Certification/5
        [HttpDelete("{id}/Certifications/{order}")]
        public async Task<ActionResult<CertificationDTO>> DeleteCertification(long id, long order)
        {
            var certification = await _context.Certifications.FindAsync(id, order);
            if (certification == null)
            {
                return NotFound();
            }

            _context.Certifications.Remove(certification);
            await _context.SaveChangesAsync();

            return CertificationToDTO(certification);
        }

        private bool CertificationExists(long id, long order)
        {
            return _context.Certifications.Any(e => (e.ResumeId == id) && (e.Order == order));
        }

        private static CertificationDTO CertificationToDTO(Certification certification) =>
            new CertificationDTO
            {
                ResumeId = certification.ResumeId,
                Order = certification.Order,
                Name = certification.Name,
                Source = certification.Source
            };

        private static Certification DTOToCertification(CertificationDTO certificationDTO) =>
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
