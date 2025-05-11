using System;
using System.Collections.Generic;

namespace Law_Office_Management.Models;

public partial class Document
{
    public int DocumentId { get; set; }

    public int? CaseId { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public DateTime? UploadDate { get; set; }

    public string? FileType { get; set; }

    public string? DocumentType { get; set; }

    public bool? IsPrivate { get; set; }

    public int? UploadedBy { get; set; }

    public virtual Case? Case { get; set; }

    public virtual User? UploadedByNavigation { get; set; }
}
