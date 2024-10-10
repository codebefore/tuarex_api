using EmployeeHierarchy.Domain.Entities;
using EmployeeHierarchy.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeHierarchy.Infrastructure.Provider
{
    public static class TestDataGenerator
    {
        public static async Task GenerateTestData(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();

            // Clear existing data
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Employees\" RESTART IDENTITY CASCADE");

            // Create multiple top-level managers (root elements)
            var ceo = new EmployeeData { FullName = "John Doe", Title = "CEO" };
            var president = new EmployeeData { FullName = "Jane Smith", Title = "President" };
            var chairperson = new EmployeeData { FullName = "Mike Johnson", Title = "Chairperson" };
            dbContext.Employees.AddRange(ceo, president, chairperson);
            await dbContext.SaveChangesAsync();

            // VPs under CEO
            var vpSales = new EmployeeData { FullName = "Alice Johnson", Title = "VP of Sales", ManagerEmployeeId = ceo.EmployeeId };
            var vpTech = new EmployeeData { FullName = "Bob Smith", Title = "VP of Technology", ManagerEmployeeId = ceo.EmployeeId };
            dbContext.Employees.AddRange(vpSales, vpTech);

            // Directors under President
            var directorOps = new EmployeeData { FullName = "Carol Williams", Title = "Director of Operations", ManagerEmployeeId = president.EmployeeId };
            var directorFinance = new EmployeeData { FullName = "David Brown", Title = "Director of Finance", ManagerEmployeeId = president.EmployeeId };
            dbContext.Employees.AddRange(directorOps, directorFinance);

            // Executives under Chairperson
            var executiveHr = new EmployeeData { FullName = "Eva Green", Title = "Executive HR", ManagerEmployeeId = chairperson.EmployeeId };
            var executiveMarketing = new EmployeeData { FullName = "Frank White", Title = "Executive Marketing", ManagerEmployeeId = chairperson.EmployeeId };
            dbContext.Employees.AddRange(executiveHr, executiveMarketing);

            await dbContext.SaveChangesAsync();

            // Create departments under VPs and Directors, with 100+ employees each
            await CreateSalesDepartment(dbContext, vpSales.EmployeeId);
            await CreateTechDepartment(dbContext, vpTech.EmployeeId);
            await CreateOperationsDepartment(dbContext, directorOps.EmployeeId);
            await CreateFinanceDepartment(dbContext, directorFinance.EmployeeId);
            await CreateHrDepartment(dbContext, executiveHr.EmployeeId);
            await CreateMarketingDepartment(dbContext, executiveMarketing.EmployeeId);

            await dbContext.SaveChangesAsync();
        }

        private static async Task CreateSalesDepartment(EmployeeDbContext dbContext, int vpSalesId)
        {
            var salesManagers = new List<EmployeeData>
    {
        new EmployeeData { FullName = "Eva Green", Title = "Sales Manager", ManagerEmployeeId = vpSalesId },
        new EmployeeData { FullName = "Frank White", Title = "Sales Manager", ManagerEmployeeId = vpSalesId }
    };
            dbContext.Employees.AddRange(salesManagers);
            await dbContext.SaveChangesAsync();

            var salesReps = new List<EmployeeData>();
            for (int i = 1; i <= 100; i++)
            {
                salesReps.Add(new EmployeeData { FullName = $"Sales Rep {i}", Title = "Sales Representative", ManagerEmployeeId = salesManagers[i % 2].EmployeeId });
            }
            dbContext.Employees.AddRange(salesReps);
            await dbContext.SaveChangesAsync();
        }

        private static async Task CreateTechDepartment(EmployeeDbContext dbContext, int vpTechId)
        {
            var techManagers = new List<EmployeeData>
    {
        new EmployeeData { FullName = "Mia Rodriguez", Title = "Development Manager", ManagerEmployeeId = vpTechId },
        new EmployeeData { FullName = "Noah Garcia", Title = "QA Manager", ManagerEmployeeId = vpTechId },
        new EmployeeData { FullName = "Olivia Martinez", Title = "Infrastructure Manager", ManagerEmployeeId = vpTechId }
    };
            dbContext.Employees.AddRange(techManagers);
            await dbContext.SaveChangesAsync();

            var techStaff = new List<EmployeeData>();
            for (int i = 1; i <= 100; i++)
            {
                techStaff.Add(new EmployeeData { FullName = $"Tech EmployeeData {i}", Title = "Developer", ManagerEmployeeId = techManagers[i % 3].EmployeeId });
            }
            dbContext.Employees.AddRange(techStaff);
            await dbContext.SaveChangesAsync();
        }

        private static async Task CreateHrDepartment(EmployeeDbContext dbContext, int vpHrId)
        {
            var hrManager = new EmployeeData { FullName = "Wendy Lopez", Title = "HR Manager", ManagerEmployeeId = vpHrId };
            dbContext.Employees.Add(hrManager);
            await dbContext.SaveChangesAsync();

            var hrStaff = new List<EmployeeData>();
            for (int i = 1; i <= 100; i++)
            {
                hrStaff.Add(new EmployeeData { FullName = $"HR EmployeeData {i}", Title = "HR Specialist", ManagerEmployeeId = hrManager.EmployeeId });
            }
            dbContext.Employees.AddRange(hrStaff);
            await dbContext.SaveChangesAsync();
        }

        private static async Task CreateMarketingDepartment(EmployeeDbContext dbContext, int vpMarketingId)
        {
            var marketingManager = new EmployeeData { FullName = "Adam Scott", Title = "Marketing Manager", ManagerEmployeeId = vpMarketingId };
            dbContext.Employees.Add(marketingManager);
            await dbContext.SaveChangesAsync();

            var marketingStaff = new List<EmployeeData>();
            for (int i = 1; i <= 100; i++)
            {
                marketingStaff.Add(new EmployeeData { FullName = $"Marketing EmployeeData {i}", Title = "Marketing Specialist", ManagerEmployeeId = marketingManager.EmployeeId });
            }
            dbContext.Employees.AddRange(marketingStaff);
            await dbContext.SaveChangesAsync();
        }

        private static async Task CreateOperationsDepartment(EmployeeDbContext dbContext, int directorOpsId)
        {
            var opsManager = new EmployeeData { FullName = "Grace Lee", Title = "Operations Manager", ManagerEmployeeId = directorOpsId };
            dbContext.Employees.Add(opsManager);
            await dbContext.SaveChangesAsync();

            var opsStaff = new List<EmployeeData>();
            for (int i = 1; i <= 100; i++)
            {
                opsStaff.Add(new EmployeeData { FullName = $"Ops EmployeeData {i}", Title = "Operations Coordinator", ManagerEmployeeId = opsManager.EmployeeId });
            }
            dbContext.Employees.AddRange(opsStaff);
            await dbContext.SaveChangesAsync();
        }

        private static async Task CreateFinanceDepartment(EmployeeDbContext dbContext, int directorFinanceId)
        {
            var financeManager = new EmployeeData { FullName = "Jack Wilson", Title = "Finance Manager", ManagerEmployeeId = directorFinanceId };
            dbContext.Employees.Add(financeManager);
            await dbContext.SaveChangesAsync();

            var financeStaff = new List<EmployeeData>();
            for (int i = 1; i <= 100; i++)
            {
                financeStaff.Add(new EmployeeData { FullName = $"Finance EmployeeData {i}", Title = "Financial Analyst", ManagerEmployeeId = financeManager.EmployeeId });
            }
            dbContext.Employees.AddRange(financeStaff);
            await dbContext.SaveChangesAsync();
        }

    }
}