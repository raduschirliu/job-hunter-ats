using cpsc_471_project.Authentication;
using cpsc_471_project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<User> userManager;
        private readonly JobHunterDBContext _context;

        public ResumesController(JobHunterDBContext context, UserManager<User> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        // GET: api/resumes
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResumeDTO>>> GetResumes()
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            List<Resume> resumes = new List<Resume>();

            // If admin, return all resumes. If user, return just your own resumes
            if (await userManager.IsInRoleAsync(user, UserRoles.Admin))
            {
                resumes = await _context.Resumes.ToListAsync();
            }
            else
            {
                resumes = await _context.Resumes.Where(r => r.CandidateId == user.Id).ToListAsync();
            }

            return resumes.Select(r => ResumeToDTO(r)).ToList();
        }

        // GET: api/resumes/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResumeDTO>> GetResume(long id)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            Resume resume;

            // Admin can get any resume, otherwise only return resume belonging to logged in user
            if (await userManager.IsInRoleAsync(user, UserRoles.Admin))
            {
                resume = await _context.Resumes.FindAsync(id);
            }
            else
            {
                resume = _context.Resumes.Where(r => r.ResumeId == id && r.CandidateId == user.Id).FirstOrDefault();
            }

            if (resume == null)
            {
                return NotFound();
            }

            return ResumeToDTO(resume);
        }

        // DELETE: api/resumes/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResumeDTO>> DeleteResume(long id)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            Resume resume = null;

            // Admin can delete any resume by ID, user can only delete their own resumes
            if (await userManager.IsInRoleAsync(user, UserRoles.Admin))
            {
                resume = await _context.Resumes.FindAsync(id);
            }
            else
            {
                resume = _context.Resumes.Where(r => r.ResumeId == id && r.CandidateId == user.Id).FirstOrDefault();
            }

            if (resume == null)
            {
                return NotFound();
            }

            _context.Resumes.Remove(resume);
            await _context.SaveChangesAsync();

            return Ok(ResumeToDTO(resume));
        }

        // POST: api/resumes
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ResumeDTO>> PostResume(ResumeDTO resumeDTO)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);

            if (resumeDTO == null)
            {
                return BadRequest();
            }

            // Assign resume to logged in user
            Resume resume = DTOToResume(resumeDTO);
            resume.CandidateId = user.Id;

            _context.Resumes.Add(resume);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostResume", resumeDTO);
        }

        // PATCH: api/resumes/{id}
        [Authorize]
        [HttpPatch("{id}")]
        public async Task<ActionResult<ResumeDTO>> PatchResume(long id, ResumeDTO resumeDTO)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);

            if (resumeDTO == null)
            {
                return BadRequest();
            }

            if (id != resumeDTO.ResumeId)
            {
                return BadRequest();
            }

            if (!ResumeExists(id))
            {
                return NotFound();
            }

            // Check to ensure you're not editing resumes for other people unless admin
            if (!(await userManager.IsInRoleAsync(user, UserRoles.Admin)))
            {
                Resume resume = await _context.Resumes.FindAsync(id);
                
                if (resume.CandidateId != user.Id)
                {
                    return Unauthorized();
                }
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
