using System;
using System.Collections.Generic;

namespace Law_Office_Management.Models;

public partial class LegalDocument
{
    public int LegalDocumentId { get; set; }

    public string? Title { get; set; }

    public string? Category { get; set; }

    public string? FilePath { get; set; }

    public DateTime? UploadedAt { get; set; }
}
