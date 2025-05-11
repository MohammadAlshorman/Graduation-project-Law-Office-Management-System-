using System;
using System.Collections.Generic;

namespace Law_Office_Management.Models;

public partial class Agency
{
    public int AgencyId { get; set; }

    public int? ClientId { get; set; }

    public int? CaseId { get; set; }

    public string? Details { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Case? Case { get; set; }

    public virtual Client? Client { get; set; }
}
