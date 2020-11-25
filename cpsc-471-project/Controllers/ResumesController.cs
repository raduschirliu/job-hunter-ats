using cpsc_471_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cpsc_471_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResumesController : Controller
    {
        private readonly JobHunterDBContext _context;

        public ResumesController(JobHunterDBContext context)
        {
            _context = context;
        }

        // GET: api/resumes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResumeDTO>>> GetResumes()
        {
            // TODO: Add auth check so that you only get your own resumes
            return (await _context.Resumes.ToListAsync()).Select(r => ResumeToDTO(r)).ToList();
        }

        // GET: api/resumes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ResumeDTO>> GetResume(long id)
        {
            Resume resume = await _context.Resumes.FindAsync(id);

            // TODO: Add auth check to make sure you can't get someone else's resumes unless admin

            if (resume == null)
            {
                return NotFound();
            }

            return ResumeToDTO(resume);
        }

        // GET: api/resumes/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResumeDTO>> DeleteResume(long id)
        {
            Resume resume = await _context.Resumes.FindAsync(id);

            // TODO: Add auth check to make sure you can't get someone else's resumes unless admin

            if (resume == null)
            {
                return NotFound();
            }

            _context.Resumes.Remove(resume);
            await _context.SaveChangesAsync();

            return Ok(ResumeToDTO(resume));
        }

        // POST: api/resumes
        [HttpPost]
        public async Task<ActionResult<ResumeDTO>> PostResume(ResumeDTO resumeDTO)
        {
            if (resumeDTO == null)
            {
                return BadRequest();
            }

            // TODO: Add auth check to ensure you're not making resumes for other people???

            _context.Resumes.Add(DTOToResume(resumeDTO));
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostResume", resumeDTO);
        }

        // PATCH: api/resumes/{id}
        [HttpPatch("{id}")]
        public async Task<ActionResult<ResumeDTO>> PatchResume(long id, ResumeDTO resumeDTO)
        {
            if (resumeDTO == null)
            {
                return BadRequest();
            }

            if (id != resumeDTO.ResumeId)
            {
                return BadRequest();
            }

            // TODO: Add auth check to ensure you're not editing resumes for other people???
            if (!ResumeExists(id))
            {
                return NotFound();
            }

            _context.Resumes.Update(DTOToResume(resumeDTO));
            await _context.SaveChangesAsync();

            return Ok(resumeDTO);
        }

        private bool ResumeExists(long resumeId)
        {
            return _context.Resumes.Any(r => r.ResumeId == resumeId);
        }

        private static ResumeDTO ResumeToDTO(Resume resume) =>
            new ResumeDTO
            {
                ResumeId = resume.ResumeId,
                Name = resume.Name,
                CandidateId = resume.CandidateId
            };

        private static Resume DTOToResume(ResumeDTO resumeDTO) =>
            new Resume
            {
                ResumeId = resumeDTO.ResumeId,
                Name = resumeDTO.Name,
                CandidateId = resumeDTO.CandidateId
            };
    }
}
