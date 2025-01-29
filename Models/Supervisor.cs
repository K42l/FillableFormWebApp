using System;
using System.Collections.Generic;

namespace FillableFormWebApp.Models;

public partial class Supervisor
{
    public int SupervisorId { get; set; }

    public int EmployeeId { get; set; }

    public virtual ICollection<DepartmentSupervisor> DepartmentSupervisors { get; set; } = new List<DepartmentSupervisor>();

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<Form> Forms { get; set; } = new List<Form>();
}
