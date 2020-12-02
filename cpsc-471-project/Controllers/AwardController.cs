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
    public class AwardController : ControllerBase
    {
        private readonly JobHunterDBContext _context;

        public AwardController(JobHunterDBContext context)
        {
            _context = context;
        }

        // GET: api/Award
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AwardDTO>>> GetAward()
        {
            var Award = await _context.Award.ToListAsync();

            // NOTE: the select function here is not querying anything
            // it is simply converting the values to another format
            // i.e. the functional programming map function is named Select in C#
            return Award.Select(x => AwardToDTO(x)).ToList();
        }

        // GET: api/Award/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AwardDTO>> GetAward(long id)
        {
            var Award = await _context.Award.FindAsync(id);

            if (Award == null)
            {
                return NotFound();
            }

            return AwardToDTO(Award);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAward(long id, Award Award)
        {
            AwardDTO AwardDTO = AwardToDTO(Award);
            Award sanitizedAward = DTOToAward(AwardDTO);
            if (id != sanitizedAward.ResumeId)
            {
                return BadRequest();
            }

            bool newAward;
            if (!AwardExists(id))
            {
                newAward = true;
                _context.Award.Add(sanitizedAward);
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
                if (!AwardExists(id))
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
                return CreatedAtAction("PutAward", new { id = sanitizedAward.ResumeId }, AwardDTO);
            }
            else
            {
                return AcceptedAtAction("PutAward", new { id = sanitizedAward.ResumeId }, AwardDTO);
            }
        }

        [HttpPost]
        public async Task<ActionResult<AwardDTO>> PostAward(Award Award)
        {
            AwardDTO AwardDTO = AwardToDTO(Award);
            Award sanitizedAward = DTOToAward(AwardDTO);
            _context.Award.Add(sanitizedAward);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostAward", new { id = sanitizedAward.ResumeId }, AwardDTO);
        }

        // DELETE: api/Award/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<AwardDTO>> DeleteAward(long id)
        {
            var Award = await _context.Award.FindAsync(id);
            if (Award == null)
            {
                return NotFound();
            }

            _context.Award.Remove(Award);
            await _context.SaveChangesAsync();

            return AwardToDTO(Award);
        }

        private bool AwardExists(long id)
        {
            return _context.Award.Any(e => e.ResumeId == id);
        }

        private static AwardDTO AwardToDTO(Award Award) =>
            new AwardDTO
            {
                ResumeId = Award.ResumeId,
                Order = Award.Order,
                Name = Award.Name,
                DateReceived = Award.DateReceived,
                value = Award.value
            };

        private static Award DTOToAward(AwardDTO AwardDTO) =>
            new Award
            {
                ResumeId = AwardDTO.ResumeId,
                Order = AwardDTO.Order,
                Name = AwardDTO.Name,
                DateReceived = AwardDTO.DateReceived,
                value = AwardDTO.value,
                Resume = null
            };
    }
}
