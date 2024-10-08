using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DMAWS_T2305M_ChuTuanLinh.Data;
using DMAWS_T2305M_ChuTuanLinh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMAWS_T2305M_ChuTuanLinh.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            return await _context.Projects.Include(p => p.ProjectEmployees).ThenInclude(pe => pe.Employees).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _context.Projects.Include(p => p.ProjectEmployees).ThenInclude(pe => pe.Employees)
                                                 .FirstOrDefaultAsync(p => p.ProjectId == id);

            if (project == null)
                return NotFound();

            return project;
        }

        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(Project project)
        {
            if (project.ProjectEndDate.HasValue && project.ProjectStartDate >= project.ProjectEndDate)
                return BadRequest("ProjectStartDate must be less than ProjectEndDate when ProjectEndDate is provided.");

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { id = project.ProjectId }, project);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, Project project)
        {
            if (id != project.ProjectId)
                return BadRequest();

            if (project.ProjectEndDate.HasValue && project.ProjectStartDate >= project.ProjectEndDate)
                return BadRequest("ProjectStartDate must be less than ProjectEndDate when ProjectEndDate is provided.");

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return NotFound();

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("search/{name}")]
        public async Task<ActionResult<IEnumerable<Project>>> SearchProjects(string name)
        {
            return await _context.Projects
                .Where(p => p.ProjectName.Contains(name))
                .Include(p => p.ProjectEmployees).ThenInclude(pe => pe.Employees)
                .ToListAsync();
        }

        [HttpGet("in-progress")]
        public async Task<ActionResult<IEnumerable<Project>>> GetInProgressProjects()
        {
            return await _context.Projects
                .Where(p => p.ProjectEndDate == null || p.ProjectEndDate > DateTime.Now)
                .Include(p => p.ProjectEmployees).ThenInclude(pe => pe.Employees)
                .ToListAsync();
        }

        [HttpGet("finished")]
        public async Task<ActionResult<IEnumerable<Project>>> GetFinishedProjects()
        {
            return await _context.Projects
                .Where(p => p.ProjectEndDate != null && p.ProjectEndDate < DateTime.Now)
                .Include(p => p.ProjectEmployees).ThenInclude(pe => pe.Employees)
                .ToListAsync();
        }
        [HttpPost("{projectId}/employees")]
        public async Task<ActionResult<ProjectEmployee>> AddEmployeeToProject(int projectId, ProjectEmployee projectEmployee)
        {

            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
            {
                return NotFound("Project not found.");
            }

            var employee = await _context.Employees.FindAsync(projectEmployee.EmployeeId);
            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

  
            projectEmployee.ProjectId = projectId;
            projectEmployee.EmployeeId = employee.EmployeeId; 

            _context.ProjectEmployees.Add(projectEmployee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProjects), new { id = projectEmployee.EmployeeId }, projectEmployee);
        }



        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }
    }
}
