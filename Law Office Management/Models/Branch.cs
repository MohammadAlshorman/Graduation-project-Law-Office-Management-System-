using System;
using System.Collections.Generic;

namespace Law_Office_Management.Models;

public partial class Branch
{
    public int BranchId { get; set; }

    public string? BranchName { get; set; }

    public string? Location { get; set; }

    public string? Phone { get; set; }
}
