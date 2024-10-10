using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeHierarchy.Domain.Entities
{
    public class EmployeeData
    {
        [Key]
        public int EmployeeId { get; set; }
        public required string FullName { get; set; }
        public required string Title { get; set; }
        public int? ManagerEmployeeId { get; set; }



        // Self-referencing foreign key
        [ForeignKey("ManagerEmployeeId")]
        public virtual EmployeeData? Manager { get; set; }
        public virtual ICollection<EmployeeData>? ManagedEmployees { get; set; }
    }
}