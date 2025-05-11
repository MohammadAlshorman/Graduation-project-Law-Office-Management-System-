using System;
using System.Collections.Generic;

namespace Law_Office_Management.Models;

public partial class FinancialReport
{
    public int ReportId { get; set; }

    public string? Title { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? ReportFile { get; set; }

    public virtual User? CreatedByNavigation { get; set; }
}
