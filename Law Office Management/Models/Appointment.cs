using System;
using System.Collections.Generic;

namespace Law_Office_Management.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int? CaseId { get; set; }

    public DateTime? Date { get; set; }

    public string? Location { get; set; }

    public string? Notes { get; set; }

    public virtual Case? Case { get; set; }
}
