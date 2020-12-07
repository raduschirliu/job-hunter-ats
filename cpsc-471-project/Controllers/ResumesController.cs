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
        // Gets a single resume, and joins all related weak entities
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResumeDetailDTO>> GetResume(long id)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);

            // Definitely not the fastest way of doing it, but after spending many hours trying to figure out group joins
            // in EF 3.0 and having it only result sadness, this will have to do for now
            ResumeDetailDTO resumeDetail = await (from resume in _context.Resumes
                                                  join candidate in _context.Users on resume.CandidateId equals candidate.Id
                                                  where resume.ResumeId == id
                                                  select new ResumeDetailDTO()
                                                  {
                                                      ResumeId = resume.ResumeId,
                                                      Name = resume.Name,
                                                      Candidate = AuthController.UserToDTO(candidate)
                                                  }).FirstOrDefaultAsync();

            if (resumeDetail == null)
            {
                return NotFound();
            }

            if (resumeDetail.Candidate.UserName != user.UserName && !await userManager.IsInRoleAsync(user, UserRoles.Admin))
            {
                return Unauthorized("Cannot view another user's resume");
            }

            // Find and join all weak entities separately
            resumeDetail.Awards = await (from award in _context.Awards where award.ResumeId == id orderby award.Order select award)
                .Select(x => AwardsController.AwardToDTO(x)).ToListAsync();

            resumeDetail.Certifications = await (from cert in _context.Certifications where cert.ResumeId == id orderby cert.Order select cert)
                .Select(x => CertificationsController.CertificationToDTO(x)).ToListAsync();

            resumeDetail.Education = await (from edu in _context.Education where edu.ResumeId == id orderby edu.Order select edu)
                .Select(x => EducationController.EducationToDTO(x)).ToListAsync();

            resumeDetail.Experience = await (from exp in _context.Experiences where exp.ResumeId == id orderby exp.Order select exp)
                .Select(x => ExperienceController.ExperienceToDTO(x)).ToListAsync();

            resumeDetail.Projects = await (from proj in _context.Projects where proj.ResumeId == id orderby proj.Order select proj)
                .Select(x => ProjectsController.ProjectToDTO(x)).ToListAsync();

            resumeDetail.Skills = await (from skill in _context.Skills where skill.ResumeId == id orderby skill.Order select skill)
                .Select(x => SkillsController.SkillToDTO(x)).ToListAsync();

            return resumeDetail;
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
            if ((resume.CandidateId != user.Id) && !(await userManager.IsInRoleAsync(user, UserRoles.Admin)))
            {
                return Unauthorized("Cannot create a resume for another candidate");
            }
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
                    return Unauthorized("Cannot change the candidate of a resume");
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
