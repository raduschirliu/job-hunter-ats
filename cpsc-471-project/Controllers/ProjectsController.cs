using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using cpsc_471_project.Authentication;
using cpsc_471_project.Models;


namespace cpsc_471_project.Controllers
{
    [Route("api/resumes")]
    [ApiController]
    public class ProjectsController : ResumeSectionController
    {
        public ProjectsController(JobHunterDBContext context, UserManager<User> userManager) : base(context, userManager) { }

        [Authorize]
        [HttpPatch("{resumeId}/projects/{order}")]
        public async Task<IActionResult> PatchProject(long resumeId, long order, ProjectDTO projectDTO)
        {
            Project sanitizedProject = DTOToProject(projectDTO);
            if (resumeId != sanitizedProject.ResumeId)
            {
                return BadRequest("resumeId in query params does not match resumeId in body");
            }

            if (order != sanitizedProject.Order)
            {
                return BadRequest("subsection order in query params does not match subsection order in body");
            }

            if (!await ResumeAccessAuthorized(sanitizedProject.ResumeId))
            {
                return GenerateResumeNotFoundError(sanitizedProject.ResumeId);
            }

            if (!ProjectExists(resumeId, order))
            {
                return BadRequest("Associated subsection does not exist");
            }
            _context.Entry(sanitizedProject).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return AcceptedAtAction("PatchProject", new { resumeId = sanitizedProject.ResumeId, order = sanitizedProject.Order }, projectDTO);
        }

        [HttpPost("{resumeId}/Projects")]
        public async Task<ActionResult<ProjectDTO>> PostProject(ProjectDTO projectDTO)
        {
            Project sanitizedProject = DTOToProject(projectDTO);

            if (!await ResumeAccessAuthorized(sanitizedProject.ResumeId))
            {
                return GenerateResumeNotFoundError(sanitizedProject.ResumeId);
            }

            _context.Projects.Add(sanitizedProject);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostProject", new { resumeId = sanitizedProject.ResumeId, order = sanitizedProject.Order }, projectDTO);
        }

        // DELETE: api/Project/5
        [HttpDelete("{resumeId}/Projects/{order}")]
        public async Task<ActionResult<ProjectDTO>> DeleteProject(long resumeId, long order)
        {
            if (!await ResumeAccessAuthorized(resumeId))
            {
                return GenerateResumeNotFoundError(resumeId);
            }

            var project = await _context.Projects.FindAsync(resumeId, order);
            if (project == null)
            {
                return NotFound("Subsection not found");
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return ProjectToDTO(project);
        }

        private bool ProjectExists(long resumeId, long order)
        {
            return _context.Projects.Any(e => (e.ResumeId == resumeId) && (e.Order == order));
        }

        public static ProjectDTO ProjectToDTO(Project project)
        {
            DateTime? inputStartDate = project.StartDate;
            if (project.StartDate.HasValue)
            {
                inputStartDate = project.StartDate.GetValueOrDefault();
            }
            DateTime? inputEndDate = project.EndDate;
            if (project.EndDate.HasValue)
            {
                inputEndDate = project.EndDate.GetValueOrDefault();
            }
            return new ProjectDTO
            {
                ResumeId = project.ResumeId,
                Order = project.Order,
                Name = project.Name,
                Description = project.Description,
                StartDate = inputStartDate,
                EndDate = inputEndDate
            };
        }

        public static Project DTOToProject(ProjectDTO projectDTO)
        {
            DateTime? inputStartDate = projectDTO.StartDate;
            if (projectDTO.StartDate.HasValue)
            {
                inputStartDate = projectDTO.StartDate.GetValueOrDefault();
            }
            DateTime? inputEndDate = projectDTO.EndDate;
            if (projectDTO.EndDate.HasValue)
            {
                inputEndDate = projectDTO.EndDate.GetValueOrDefault();
            }
            return new Project
            {
                ResumeId = projectDTO.ResumeId,
                Order = projectDTO.Order,
                Name = projectDTO.Name,
                Description = projectDTO.Description,
                StartDate = inputStartDate,
                EndDate = inputEndDate,
                Resume = null
            };
        }
            
    }
}
