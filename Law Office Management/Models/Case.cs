using System;
using System.Collections.Generic;

namespace Law_Office_Management.Models;

public partial class Case
{
    public int CaseId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? ClientId { get; set; }

    public int? LawyerId { get; set; }

    public virtual ICollection<Agency> Agencies { get; set; } = new List<Agency>();

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Client? Client { get; set; }

    public virtual ICollection<CourtSession> CourtSessions { get; set; } = new List<CourtSession>();

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual Lawyer? Lawyer { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Verdict> Verdicts { get; set; } = new List<Verdict>();
}
