using EmployeeHierarchy.Domain.Dtos;
using EmployeeHierarchy.Domain.Entities;
using EmployeeHierarchy.Domain.Interfaces;
using EmployeeHierarchy.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace EmployeeHierarchy.Infrastructure.Repos
{
    public class EmployeeRepositoryWithAdonet : IEmployeeRepository
    {
        private readonly EmployeeDbContext _context;

        public EmployeeRepositoryWithAdonet(EmployeeDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<int> CreateOrUpdateEmployee(Employee employee)
        {
            var employeeData = await _context.Employees.FindAsync(employee.EmployeeId);
            if (employeeData == null)
            {
                employeeData = new EmployeeData
                {
                    FullName = employee.FullName,
                    Title = employee.Title,
                    ManagerEmployeeId = employee.ManagerEmployeeId
                };
                _context.Employees.Add(employeeData);
            }
            else
            {
                employeeData.FullName = employee.FullName;
                employeeData.Title = employee.Title;
                employeeData.ManagerEmployeeId = employee.ManagerEmployeeId;
                _context.Employees.Update(employeeData);
            }

            await _context.SaveChangesAsync();
            return employeeData.EmployeeId;
        }

        public async Task<bool> DeleteEmployee(int id)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee != null)
            {

                if (_context.Employees.Any(x => x.ManagerEmployeeId == employee.EmployeeId))
                {
                    var subEmployees = _context.Employees.Where(x => x.ManagerEmployeeId == employee.EmployeeId).ToList();
                    foreach (var subEmployee in subEmployees)
                    {
                        subEmployee.ManagerEmployeeId = employee.ManagerEmployeeId;
                    }
                    _context.UpdateRange(subEmployees);
                }
                _context.Employees.Remove(employee);//only this object
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<Employee> GetEmployeeById(int id)
        {
            var employeeData = _context.Employees
            .FirstOrDefault(e => e.EmployeeId == id);
            var employee = new Employee(employeeData);
            employee.ManagedEmployees = await LoadManagedEmployeesRecursively(id);
            return employee;
        }

        public async Task<IEnumerable<Employee>> GetEmployees()
        {
            // For each subordinate, fetch their subordinates recursively
            return await LoadManagedEmployeesRecursively(null);
        }

        private async Task<List<Employee>> LoadManagedEmployeesRecursively(int? id)
        {
            var managedEmployees = await _context.Employees
             .Where(e => e.ManagerEmployeeId == id)
             .Select(x => new Employee(x))
             .ToListAsync();

            foreach (var employee in managedEmployees)
                employee.ManagedEmployees = await LoadManagedEmployeesRecursively(employee.EmployeeId);

            return managedEmployees;
        }
    }
}