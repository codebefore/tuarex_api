using EmployeeHierarchy.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace EmployeeHierarchy.Domain.Dtos
{
    public class Employee
    {
        public Employee()
        {

        }
        public Employee(EmployeeData employeeData)
        {
            EmployeeId = employeeData.EmployeeId;
            FullName = employeeData.FullName;
            Title = employeeData.Title;
            ManagerEmployeeId = employeeData.ManagerEmployeeId;
        }
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Title { get; set; }
        public int? ManagerEmployeeId { get; set; }
        public List<Employee>? ManagedEmployees { get; set; }
    }

    public class EmployeeRequest
    {
        public int? EmployeeId { get; set; }

        [MinLength(1)]
        [MaxLength(250)]
        public required string FullName { get; set; }
        [MinLength(1)]
        [MaxLength(250)]
        public required string Title { get; set; }
        public int? ManagerEmployeeId { get; set; }

        public Employee GetEmployee()
        {
            return new Employee { EmployeeId = EmployeeId ?? 0, FullName = FullName, Title = Title, ManagerEmployeeId = ManagerEmployeeId };
        }
    }

}