using System;
using System.Collections.Generic;

namespace FillableFormWebApp.Models;

public partial class Department
{
    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; } = null!;

    public virtual ICollection<DepartmentSupervisor> DepartmentSupervisors { get; set; } = new List<DepartmentSupervisor>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
