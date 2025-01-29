using System;
using System.Collections.Generic;
using FillableFormWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FillableFormWebApp.Data;

public partial class FillableFormWebAppContext : DbContext
{
    public FillableFormWebAppContext(DbContextOptions<FillableFormWebAppContext> options)
        : base(options)
    {
    }
    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<DepartmentSupervisor> DepartmentSupervisors { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Form> Forms { get; set; }

    public virtual DbSet<FormType> FormTypes { get; set; }

    public virtual DbSet<Supervisor> Supervisors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasIndex(e => e.DepartmentName, "IX_Departments_department_name").IsUnique();

            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.DepartmentName)
                .HasColumnType("nvarchar(100)")
                .HasColumnName("department_name");
        });

        modelBuilder.Entity<DepartmentSupervisor>(entity =>
        {
            entity.Property(e => e.DepartmentSupervisorId).HasColumnName("departmentSupervisor_id");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.SupervisorId).HasColumnName("supervisor_id");

            entity.HasOne(d => d.Department).WithMany(p => p.DepartmentSupervisors)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Supervisor).WithMany(p => p.DepartmentSupervisors)
                .HasForeignKey(d => d.SupervisorId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasIndex(e => e.Ssn, "IX_Employees_ssn").IsUnique();

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.Email)
                .HasColumnType("nvarchar(100)")
                .HasColumnName("email");
            entity.Property(e => e.EmployeeName)
                .HasColumnType("nvarchar(250)")
                .HasColumnName("employee_name");
            entity.Property(e => e.Ssn)
                .HasColumnType("INT")
                .HasColumnName("ssn");

            entity.HasOne(d => d.Department).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Form>(entity =>
        {
            entity.Property(e => e.FormId).HasColumnName("form_id");
            entity.Property(e => e.AdditionalRemarks)
                .HasColumnType("nvarchar(450)")
                .HasColumnName("additionalRemarks");
            entity.Property(e => e.Dates)
                .HasColumnType("nvarchar(100)")
                .HasColumnName("dates");
            entity.Property(e => e.Decision)
                .HasColumnType("nvarchar(50)")
                .HasColumnName("decision");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.FormDate)
                .HasColumnType("datetime")
                .HasColumnName("form_date");
            entity.Property(e => e.FormTypeId).HasColumnName("formType_id");
            entity.Property(e => e.Justification)
                .HasColumnType("nvarchar(450)")
                .HasColumnName("justification");
            entity.Property(e => e.Reason)
                .HasColumnType("nvarchar(250)")
                .HasColumnName("reason");
            entity.Property(e => e.Status)
                .HasColumnType("nvarchar(7)")
                .HasColumnName("status");
            entity.Property(e => e.SupervisorId).HasColumnName("supervisor_id");
            entity.Property(e => e.TypeOfLeave)
                .HasColumnType("nvarchar(64)")
                .HasColumnName("typeOfLeave");
            entity.Property(e => e.Other)
                .HasColumnType("nvarchar(70)")
                .HasColumnName("other");

            entity.HasOne(d => d.Employee).WithMany(p => p.Forms)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.FormType).WithMany(p => p.Forms)
                .HasForeignKey(d => d.FormTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Supervisor).WithMany(p => p.Forms)
                .HasForeignKey(d => d.SupervisorId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FormType>(entity =>
        {
            entity.HasIndex(e => e.FormTypeName, "IX_FormTypes_formType_name").IsUnique();

            entity.Property(e => e.FormTypeId).HasColumnName("formType_id");
            entity.Property(e => e.FormTypeName)
                .HasColumnType("nvarchar(100)")
                .HasColumnName("formType_name");
        });

        modelBuilder.Entity<Supervisor>(entity =>
        {
            entity.Property(e => e.SupervisorId).HasColumnName("supervisor_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");

            entity.HasOne(d => d.Employee).WithMany(p => p.Supervisors)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
