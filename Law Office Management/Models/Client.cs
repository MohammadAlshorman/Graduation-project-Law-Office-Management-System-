using System;
using System.Collections.Generic;

namespace Law_Office_Management.Models;

public partial class Client
{
    public int ClientId { get; set; }

    public string? FullName { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? NationalId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Agency> Agencies { get; set; } = new List<Agency>();

    public virtual ICollection<Case> Cases { get; set; } = new List<Case>();

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
