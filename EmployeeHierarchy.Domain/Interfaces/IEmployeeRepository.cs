using EmployeeHierarchy.Domain.Dtos;

namespace EmployeeHierarchy.Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<int> CreateOrUpdateEmployee(Employee employee);
        Task<bool> DeleteEmployee(int id);
        Task<IEnumerable<Employee>> GetEmployees();
        Task<Employee> GetEmployeeById(int id);
    }
}