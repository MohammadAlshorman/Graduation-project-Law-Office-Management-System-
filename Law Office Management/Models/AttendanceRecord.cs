using System;
using System.Collections.Generic;

namespace Law_Office_Management.Models;

public partial class AttendanceRecord
{
    public int AttendanceId { get; set; }

    public DateTime? CheckInTime { get; set; }

    public DateTime? CheckOutTime { get; set; }

    public int? LawyerId { get; set; }

    public string? Notes { get; set; }

    public virtual Lawyer? Lawyer { get; set; }
}
