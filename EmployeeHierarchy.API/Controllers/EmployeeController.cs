using EmployeeHierarchy.Domain.Dtos;
using EmployeeHierarchy.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeHierarchy.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        [HttpPut]
        public async Task<IActionResult> CreateOrUpdateEmployee([FromBody] EmployeeRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employeeId = await _employeeRepository.CreateOrUpdateEmployee(model.GetEmployee());
            return Ok(employeeId);
        }

        [HttpDelete("{employeeId}")]
        public async Task<IActionResult> DeleteEmployee(int employeeId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (employeeId <= 0)
                return BadRequest("EmployeeId must be bigger than zero");

            var isDeleted = await _employeeRepository.DeleteEmployee(employeeId);
            if (!isDeleted)
                return NoContent();
            else return Ok();
        }

        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetEmployeeById(int employeeId)
        {
            var employee = await _employeeRepository.GetEmployeeById(employeeId);
            if (employee == null)
                return NotFound($"Employee with ID {employeeId} not found.");
            return Ok(employee);
        }

        [HttpGet("/Employees")]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _employeeRepository.GetEmployees();
            return Ok(employees);
        }
    }
}