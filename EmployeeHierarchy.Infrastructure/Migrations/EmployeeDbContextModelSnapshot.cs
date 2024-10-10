﻿// <auto-generated />
using EmployeeHierarchy.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace EmployeeHierarchy.Infrastructure.Migrations
{
    [DbContext(typeof(EmployeeDbContext))]
    partial class EmployeeDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("EmployeeHierarchy.Domain.Employee", b =>
                {
                    b.Property<int>("EmployeeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("EmployeeId"));

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("ManagerEmployeeId")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("EmployeeId");

                    b.HasIndex("ManagerEmployeeId");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("EmployeeHierarchy.Domain.Employee", b =>
                {
                    b.HasOne("EmployeeHierarchy.Domain.Employee", null)
                        .WithMany("ManagedEmployees")
                        .HasForeignKey("ManagerEmployeeId");
                });

            modelBuilder.Entity("EmployeeHierarchy.Domain.Employee", b =>
                {
                    b.Navigation("ManagedEmployees");
                });
#pragma warning restore 612, 618
        }
    }
}
