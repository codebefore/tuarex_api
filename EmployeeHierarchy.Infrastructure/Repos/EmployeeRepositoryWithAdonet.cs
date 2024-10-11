using EmployeeHierarchy.Domain.Dtos;
using EmployeeHierarchy.Domain.Entities;
using EmployeeHierarchy.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32.SafeHandles;
using Npgsql;
using System.Data;

namespace EmployeeHierarchy.Infrastructure.Repos
{
    public class EmployeeRepositoryWithAdonet(IConfiguration configuration) : IEmployeeRepository
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection");
        public async Task<int> CreateOrUpdateEmployee(Employee employee)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        using var command = new NpgsqlCommand(
                            @"INSERT INTO ""Employees"" (""EmployeeId"",""FullName"", ""Title"", ""ManagerEmployeeId"")
                  VALUES (@EmployeeId,@FullName, @Title, @ManagerEmployeeId)
                  ON CONFLICT (""EmployeeId"") DO UPDATE
                  SET ""FullName"" = @FullName, ""Title"" = @Title, ""ManagerEmployeeId"" = @ManagerEmployeeId
                  RETURNING ""EmployeeId""", connection);

                        command.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId == 0 ? (object)DBNull.Value : employee.EmployeeId);
                        command.Parameters.AddWithValue("@FullName", employee.FullName);
                        command.Parameters.AddWithValue("@Title", employee.Title);
                        command.Parameters.AddWithValue("@ManagerEmployeeId", employee.ManagerEmployeeId ?? (object)DBNull.Value);

                        //if (employee.EmployeeId != 0)
                        //    command.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId);

                        var result = await command.ExecuteScalarAsync();
                        await transaction.CommitAsync();
                        await connection.CloseAsync();
                        return Convert.ToInt32(result);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        await connection.CloseAsync();
                        throw;
                    }
                }
            }
        }

        public async Task<bool> DeleteEmployee(int id)
        {

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        // Update managed employees
                        using (var updateCommand = new NpgsqlCommand(
                            @"UPDATE ""Employees""
                      SET ""ManagerEmployeeId"" = (SELECT ""ManagerEmployeeId"" FROM ""Employees"" WHERE ""EmployeeId"" = @EmployeeId)
                      WHERE ""ManagerEmployeeId"" = @EmployeeId", connection, transaction))
                        {
                            updateCommand.Parameters.AddWithValue("@EmployeeId", id);
                            await updateCommand.ExecuteNonQueryAsync();
                        }

                        // Delete the employee
                        using (var deleteCommand = new NpgsqlCommand(
                            @"DELETE FROM ""Employees"" WHERE ""EmployeeId"" = @EmployeeId", connection, transaction))
                        {
                            deleteCommand.Parameters.AddWithValue("@EmployeeId", id);
                            var rowsAffected = await deleteCommand.ExecuteNonQueryAsync();

                            await transaction.CommitAsync();
                            return rowsAffected > 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        await connection.CloseAsync();
                        throw;
                    }
                }
            }
        }

        public async Task<Employee> GetEmployeeById(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using var command = new NpgsqlCommand(
                    @"SELECT ""EmployeeId"", ""FullName"", ""Title"", ""ManagerEmployeeId""
                  FROM ""Employees""
                  WHERE ""EmployeeId"" = @EmployeeId", connection);
                command.Parameters.AddWithValue("@EmployeeId", id);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var employee = new Employee
                    {
                        EmployeeId = reader.GetInt32(0),
                        FullName = reader.GetString(1),
                        Title = reader.GetString(2),
                        ManagerEmployeeId = reader.IsDBNull(3) ? null : reader.GetInt32(3)
                    };
                    employee.ManagedEmployees = await LoadManagedEmployeesRecursively(id);
                    await connection.CloseAsync();
                    return employee;
                }
                await connection.CloseAsync();
                return null;
            }
        }

        public async Task<IEnumerable<Employee>> GetEmployees()
        {
            return await LoadManagedEmployeesRecursively(null);
        }

        private async Task<List<Employee>> LoadManagedEmployeesRecursively(int? managerId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using var command = new NpgsqlCommand(
                    @"SELECT ""EmployeeId"", ""FullName"", ""Title"", ""ManagerEmployeeId""
                  FROM ""Employees""
                  WHERE ""ManagerEmployeeId"" IS NOT DISTINCT FROM @ManagerId", connection);
                command.Parameters.AddWithValue("@ManagerId", managerId ?? (object)DBNull.Value);

                using var reader = await command.ExecuteReaderAsync();
                var employees = new List<Employee>();

                while (await reader.ReadAsync())
                {
                    var employee = new Employee
                    {
                        EmployeeId = reader.GetInt32(0),
                        FullName = reader.GetString(1),
                        Title = reader.GetString(2),
                        ManagerEmployeeId = reader.IsDBNull(3) ? null : reader.GetInt32(3)
                    };
                    employees.Add(employee);
                }

                foreach (var employee in employees)
                {
                    employee.ManagedEmployees = await LoadManagedEmployeesRecursively(employee.EmployeeId);
                }
                await connection.CloseAsync();
                return employees;
            }
        }
    }
}