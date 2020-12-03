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
    public class CertificationsController : ControllerBase
    {
        private readonly JobHunterDBContext _context;

        public CertificationsController(JobHunterDBContext context)
        {
            _context = context;
        }

        // GET: api/Certification
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CertificationDTO>>> GetCertification()
        {
            var Certification = await _context.Certifications.ToListAsync();

            // NOTE: the select function here is not querying anything
            // it is simply converting the values to another format
            // i.e. the functional programming map function is named Select in C#
            return Certification.Select(x => CertificationToDTO(x)).ToList();
        }

        // GET: api/Certification/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CertificationDTO>> GetCertification(long id)
        {
            var Certification = await _context.Certifications.FindAsync(id);

            if (Certification == null)
            {
                return NotFound();
            }

            return CertificationToDTO(Certification);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCertification(long id, Certification Certification)
        {
            CertificationDTO CertificationDTO = CertificationToDTO(Certification);
            Certification sanitizedCertification = DTOToCertification(CertificationDTO);
            if (id != sanitizedCertification.ResumeId)
            {
                return BadRequest();
            }

            bool newCertification;
            if (!CertificationExists(id))
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
                if (!CertificationExists(id))
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
                return CreatedAtAction("PutCertification", new { id = sanitizedCertification.ResumeId }, CertificationDTO);
            }
            else
            {
                return AcceptedAtAction("PutCertification", new { id = sanitizedCertification.ResumeId }, CertificationDTO);
            }
        }

        [HttpPost]
        public async Task<ActionResult<CertificationDTO>> PostCertification(Certification Certification)
        {
            CertificationDTO CertificationDTO = CertificationToDTO(Certification);
            Certification sanitizedCertification = DTOToCertification(CertificationDTO);
            _context.Certifications.Add(sanitizedCertification);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostCertification", new { id = sanitizedCertification.ResumeId }, CertificationDTO);
        }

        // DELETE: api/Certification/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CertificationDTO>> DeleteCertification(long id)
        {
            var Certification = await _context.Certifications.FindAsync(id);
            if (Certification == null)
            {
                return NotFound();
            }

            _context.Certifications.Remove(Certification);
            await _context.SaveChangesAsync();

            return CertificationToDTO(Certification);
        }

        private bool CertificationExists(long id)
        {
            return _context.Certifications.Any(e => e.ResumeId == id);
        }

        private static CertificationDTO CertificationToDTO(Certification Certification) =>
            new CertificationDTO
            {
                ResumeId = Certification.ResumeId,
                Order = Certification.Order,
                Name = Certification.Name,
                Source = Certification.Source
            };

        private static Certification DTOToCertification(CertificationDTO CertificationDTO) =>
            new Certification
            {
               ResumeId = CertificationDTO.ResumeId,
                Order = CertificationDTO.Order,
                Name = CertificationDTO.Name,
                Source = CertificationDTO.Source,
                Resume = null
                //Had user = null here Idk what that is for ask radu
            };
    }
}
