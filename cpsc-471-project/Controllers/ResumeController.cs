using cpsc_471_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cpsc_471_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResumeController : Controller
    {
        private readonly JobHunterDBContext _context;

        public ResumeController(JobHunterDBContext context)
        {
            _context = context;
        }

        // GET: api/Resume
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResumeDTO>>> GetAllResumes()
        {
            // TODO: Add auth check so that you only get your own resumes
            return (await _context.Resume.ToListAsync()).Select(r => ResumeToDTO(r)).ToList();
        }

        // GET: api/Resume/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ResumeDTO>> GetResume(long id)
        {
            Resume resume = await _context.Resume.FindAsync(id);

            // TODO: Add auth check to make sure you can't get someone else's resumes unless admin

            if (resume == null)
            {
                return NotFound();
            }

            return ResumeToDTO(resume);
        }

        // GET: api/Resume/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResumeDTO>> DeleteResume(long id)
        {
            Resume resume = await _context.Resume.FindAsync(id);

            // TODO: Add auth check to make sure you can't get someone else's resumes unless admin

            if (resume == null)
            {
                return NotFound();
            }

            return ResumeToDTO(resume);
        }

        // POST: api/Resume
        [HttpPost]
        public async Task<ActionResult<ResumeDTO>> PostResume(ResumeDTO resumeDTO)
        {
            if (resumeDTO == null)
            {
                return BadRequest();
            }

            // TODO: Add auth check to ensure you're not making resumes for other people???

            _context.Resume.Add(DTOToResume(resumeDTO));
            await _context.SaveChangesAsync();

            return Ok(resumeDTO);
        }

        // PATCH: api/Resume/{id}
        [HttpPatch]
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

            Resume oldResume = await _context.Resume.FindAsync(id);

            if (oldResume == null)
            {
                return NotFound();
            }

            await _context.SaveChangesAsync();

            return Ok(resumeDTO);
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
