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
    public class ProjectsController : ControllerBase
    {
        private readonly JobHunterDBContext _context;

        public ProjectsController(JobHunterDBContext context)
        {
            _context = context;
        }

        [HttpPut("{id}/Projects/{order}")]
        public async Task<IActionResult> PutProject(long id, long order, ProjectDTO projectDTO)
        {
            Project sanitizedProject = DTOToProject(projectDTO);
            if (id != sanitizedProject.ResumeId)
            {
                return BadRequest();
            }

            if (order != sanitizedProject.Order)
            {
                return BadRequest();
            }

            bool newProject;
            if (!ProjectExists(id, order))
            {
                newProject = true;
                _context.Projects.Add(sanitizedProject);
            }
            else
            {
                newProject = false;
                _context.Entry(sanitizedProject).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id, order))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            if (newProject)
            {
                return CreatedAtAction("PutProject", new { id = sanitizedProject.ResumeId, order = sanitizedProject.Order }, projectDTO);
            }
            else
            {
                return AcceptedAtAction("PutProject", new { id = sanitizedProject.ResumeId, order = sanitizedProject.Order }, projectDTO);
            }
        }

        [HttpPost("{id}/Projects")]
        public async Task<ActionResult<ProjectDTO>> PostProject(ProjectDTO projectDTO)
        {
            Project sanitizedProject = DTOToProject(projectDTO);
            _context.Projects.Add(sanitizedProject);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostProject", new { id = sanitizedProject.ResumeId, order = sanitizedProject.Order }, projectDTO);
        }

        // DELETE: api/Project/5
        [HttpDelete("{id}/Projects/{order}")]
        public async Task<ActionResult<ProjectDTO>> DeleteProject(long id, long order)
        {
            var project = await _context.Projects.FindAsync(id, order);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return ProjectToDTO(project);
        }

        private bool ProjectExists(long id, long order)
        {
            return _context.Projects.Any(e => (e.ResumeId == id) && (e.Order == order));
        }

        private static ProjectDTO ProjectToDTO(Project project) =>
            new ProjectDTO
            {
                ResumeId = project.ResumeId,
                Order = project.Order,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate
            };

        private static Project DTOToProject(ProjectDTO projectDTO) =>
            new Project
            {
                ResumeId = projectDTO.ResumeId,
                Order = projectDTO.Order,
                Name = projectDTO.Name,
                Description = projectDTO.Description,
                StartDate = projectDTO.StartDate,
                EndDate = projectDTO.EndDate,
                Resume = null
            };
    }
}
