using System;
using System.Collections.Generic;

namespace Law_Office_Management.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int? ClientId { get; set; }

    public decimal? Amount { get; set; }

    public DateOnly? DueDate { get; set; }

    public bool? IsPaid { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Client? Client { get; set; }
}
