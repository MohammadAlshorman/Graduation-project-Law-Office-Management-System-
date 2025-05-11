using System;
using System.Collections.Generic;

namespace Law_Office_Management.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string? FullName { get; set; }

    public string? Position { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Branch { get; set; }
}
