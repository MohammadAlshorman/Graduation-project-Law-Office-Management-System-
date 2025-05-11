using System;
using System.Collections.Generic;

namespace Law_Office_Management.Models;

public partial class Verdict
{
    public int VerdictId { get; set; }

    public int? CaseId { get; set; }

    public string? Summary { get; set; }

    public DateTime? VerdictDate { get; set; }

    public virtual Case? Case { get; set; }
}
