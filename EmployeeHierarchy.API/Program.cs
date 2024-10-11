using EmployeeHierarchy.Domain.Interfaces;
using EmployeeHierarchy.Infrastructure.Context;
using EmployeeHierarchy.Infrastructure.Provider; 
using EmployeeHierarchy.Infrastructure.Repos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//if needed use this one for ef core 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<EmployeeDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepositoryWithAdonet>();

var app = builder.Build();

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