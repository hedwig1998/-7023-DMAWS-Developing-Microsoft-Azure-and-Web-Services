using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMAWS_T2305M_ChuTuanLinh.Data;
using DMAWS_T2305M_ChuTuanLinh.Models;

namespace DMAWS_T2305M_ChuTuanLinh.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            return await _context.Employees.Include(e => e.ProjectEmployees).ThenInclude(pe => pe.Projects).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees.Include(e => e.ProjectEmployees).ThenInclude(pe => pe.Projects)
                                                   .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
                return NotFound();

            return employee;
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            if (DateTime.Now.Year - employee.EmployeeDOB.Year < 16)
                return BadRequest("Employee must be over 16 years old.");

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeId }, employee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            if (id != employee.EmployeeId)
                return BadRequest();

            if (DateTime.Now.Year - employee.EmployeeDOB.Year < 16)
                return BadRequest("Employee must be over 16 years old.");

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound();

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("search/{name}")]
        public async Task<ActionResult<IEnumerable<Employee>>> SearchEmployees(string name)
        {
            return await _context.Employees
                .Where(e => e.EmployeeName.Contains(name))
                .Include(e => e.ProjectEmployees).ThenInclude(pe => pe.Projects)
                .ToListAsync();
        }

        [HttpGet("dob")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesByDOB(DateTime fromDate, DateTime toDate)
        {
            return await _context.Employees
                .Where(e => e.EmployeeDOB >= fromDate && e.EmployeeDOB <= toDate)
                .Include(e => e.ProjectEmployees).ThenInclude(pe => pe.Projects)
                .ToListAsync();
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
