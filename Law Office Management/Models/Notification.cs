using System;
using System.Collections.Generic;

namespace Law_Office_Management.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int? UserId { get; set; }

    public string? Message { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Title { get; set; }

    public string? Type { get; set; }

    public int? RelatedCaseId { get; set; }

    public virtual Case? RelatedCase { get; set; }

    public virtual User? User { get; set; }
}
