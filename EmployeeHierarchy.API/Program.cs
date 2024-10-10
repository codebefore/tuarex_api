using Microsoft.EntityFrameworkCore;
using EmployeeHierarchy.Domain.Interfaces;
using EmployeeHierarchy.Infrastructure.Context;
using EmployeeHierarchy.Infrastructure.Repos;
using EmployeeHierarchy.Infrastructure.Provider; // Add this line

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<EmployeeDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepositoryWithAdonet>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Apply migrations and generate test data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();


    var databaseExists = await dbContext.Database.CanConnectAsync();
    if (!databaseExists)
    {
        await dbContext.Database.MigrateAsync();
        await TestDataGenerator.GenerateTestData(app.Services);
    }

}

app.Run();