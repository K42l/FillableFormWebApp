using System;
using System.Collections.Generic;

namespace FillableFormWebApp.Models;

public partial class FormType
{
    public int FormTypeId { get; set; }

    public string FormTypeName { get; set; } = null!;

    public virtual ICollection<Form> Forms { get; set; } = new List<Form>();
}
