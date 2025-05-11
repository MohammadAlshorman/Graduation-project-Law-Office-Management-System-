using System;
using System.Collections.Generic;

namespace Law_Office_Management.Models;

public partial class Lawyer
{
    public int LawyerId { get; set; }

    public string? FullName { get; set; }

    public string? Specialty { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public bool IsActive { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<AppTask> AppTasks { get; set; } = new List<AppTask>();

    public virtual ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();

    public virtual ICollection<Case> Cases { get; set; } = new List<Case>();

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual User? User { get; set; }
}
