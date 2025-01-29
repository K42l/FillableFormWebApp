using System;
using System.Collections.Generic;

namespace FillableFormWebApp.Models;

public partial class DepartmentSupervisor
{
    public int DepartmentSupervisorId { get; set; }

    public int SupervisorId { get; set; }

    public int DepartmentId { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual Supervisor Supervisor { get; set; } = null!;
}
