using System;
using System.Collections.Generic;

namespace Law_Office_Management.Models;

public partial class AppTask
{
    public int TaskId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? AssignedTo { get; set; }

    public DateTime? DueDate { get; set; }

    public bool? IsCompleted { get; set; }

    public int? LawyerId { get; set; }

    public virtual User? AssignedToNavigation { get; set; }

    public virtual Lawyer? Lawyer { get; set; }
}
