using System;
using System.Collections.Generic;

namespace Law_Office_Management.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? Username { get; set; }

    public string? PasswordHash { get; set; }

    public string? Role { get; set; }

    public bool IsActive { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<AppTask> AppTasks { get; set; } = new List<AppTask>();

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual ICollection<FinancialReport> FinancialReports { get; set; } = new List<FinancialReport>();

    public virtual ICollection<Lawyer> Lawyers { get; set; } = new List<Lawyer>();

    public virtual ICollection<Message> MessageReceivers { get; set; } = new List<Message>();

    public virtual ICollection<Message> MessageSenders { get; set; } = new List<Message>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
