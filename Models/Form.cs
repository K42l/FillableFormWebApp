using System;
using System.Collections.Generic;

namespace FillableFormWebApp.Models;

public partial class Form
{
    public int FormId { get; set; }

    public int EmployeeId { get; set; }

    public int SupervisorId { get; set; }

    public int FormTypeId { get; set; }

    public DateTime? FormDate { get; set; }

    public string? Justification { get; set; }

    public string? Dates { get; set; }

    public string? TypeOfLeave { get; set; }
    
    public string? Other { get; set; }

    public string? AdditionalRemarks { get; set; }

    public string? Decision { get; set; }

    public string? Reason { get; set; }

    public string? Status { get; set; } = null!;

    public virtual Employee? Employee { get; set; } = null!;

    public virtual FormType? FormType { get; set; } = null!;

    public virtual Supervisor? Supervisor { get; set; } = null!;
}
