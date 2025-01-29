using System;
using System.Collections.Generic;

namespace FillableFormWebApp.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int Ssn { get; set; }

    public int DepartmentId { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<Form> Forms { get; set; } = new List<Form>();

    public virtual ICollection<Supervisor> Supervisors { get; set; } = new List<Supervisor>();
}
