using System;
using System.Collections.Generic;

namespace Law_Office_Management.Models;

public partial class Contract
{
    public int ContractId { get; set; }

    public string? Title { get; set; }

    public int? ClientId { get; set; }

    public int? LawyerId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? FilePath { get; set; }

    public virtual Client? Client { get; set; }

    public virtual Lawyer? Lawyer { get; set; }
}
