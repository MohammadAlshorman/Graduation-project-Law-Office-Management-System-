using System;
using System.Collections.Generic;

namespace Law_Office_Management.Models;

public partial class ContactMessage
{
    public int Id { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Subject { get; set; }

    public string? MessageText { get; set; }

    public DateTime? SentAt { get; set; }
}
