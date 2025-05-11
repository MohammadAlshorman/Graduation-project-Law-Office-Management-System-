using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Law_Office_Management.Models;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Agency> Agencies { get; set; }

    public virtual DbSet<AppTask> AppTasks { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AttendanceRecord> AttendanceRecords { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Case> Cases { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<ContactMessage> ContactMessages { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<CourtSession> CourtSessions { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<FinancialReport> FinancialReports { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Lawyer> Lawyers { get; set; }

    public virtual DbSet<LegalDocument> LegalDocuments { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Verdict> Verdicts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=MOHAMMAD-ALSHOR;Database=LawOfficeManagement;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agency>(entity =>
        {
            entity.HasKey(e => e.AgencyId).HasName("PK__Agencies__95C546DB7740D6DF");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Case).WithMany(p => p.Agencies)
                .HasForeignKey(d => d.CaseId)
                .HasConstraintName("FK__Agencies__CaseId__6A30C649");

            entity.HasOne(d => d.Client).WithMany(p => p.Agencies)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK__Agencies__Client__693CA210");
        });

        modelBuilder.Entity<AppTask>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__Tasks__7C6949B1AF773EB8");

            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.IsCompleted).HasDefaultValue(false);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.AppTasks)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK__Tasks__AssignedT__5DCAEF64");

            entity.HasOne(d => d.Lawyer).WithMany(p => p.AppTasks)
                .HasForeignKey(d => d.LawyerId)
                .HasConstraintName("FK_AppTasks_Lawyers");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCC24F345597");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Location).HasMaxLength(200);

            entity.HasOne(d => d.Case).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.CaseId)
                .HasConstraintName("FK__Appointme__CaseI__5AEE82B9");
        });

        modelBuilder.Entity<AttendanceRecord>(entity =>
        {
            entity.HasKey(e => e.AttendanceId).HasName("PK__Attendan__8B69261CD7FA841D");

            entity.Property(e => e.CheckInTime).HasColumnType("datetime");
            entity.Property(e => e.CheckOutTime).HasColumnType("datetime");

            entity.HasOne(d => d.Lawyer).WithMany(p => p.AttendanceRecords)
                .HasForeignKey(d => d.LawyerId)
                .HasConstraintName("FK_AttendanceRecords_Lawyers");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.BranchId).HasName("PK__Branches__A1682FC5D4FAC2D7");

            entity.Property(e => e.BranchName).HasMaxLength(100);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<Case>(entity =>
        {
            entity.HasKey(e => e.CaseId).HasName("PK__Cases__6CAE524C34F12A17");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Client).WithMany(p => p.Cases)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK__Cases__ClientId__4CA06362");

            entity.HasOne(d => d.Lawyer).WithMany(p => p.Cases)
                .HasForeignKey(d => d.LawyerId)
                .HasConstraintName("FK__Cases__LawyerId__4D94879B");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("PK__Clients__E67E1A24B437AAF0");

            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.NationalId)
                .HasMaxLength(50)
                .HasColumnName("NationalID");
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ContactM__3214EC07C4872E84");

            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Subject).HasMaxLength(200);
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK__Contract__C90D34697195F773");

            entity.Property(e => e.FilePath).HasMaxLength(300);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Client).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK__Contracts__Clien__571DF1D5");

            entity.HasOne(d => d.Lawyer).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.LawyerId)
                .HasConstraintName("FK__Contracts__Lawye__5812160E");
        });

        modelBuilder.Entity<CourtSession>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PK__CourtSes__C9F492908E41C404");

            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.SessionDate).HasColumnType("datetime");

            entity.HasOne(d => d.Case).WithMany(p => p.CourtSessions)
                .HasForeignKey(d => d.CaseId)
                .HasConstraintName("FK__CourtSess__CaseI__5070F446");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("PK__Document__1ABEEF0FF8111096");

            entity.Property(e => e.DocumentType).HasMaxLength(100);
            entity.Property(e => e.FileName).HasMaxLength(200);
            entity.Property(e => e.FilePath).HasMaxLength(300);
            entity.Property(e => e.FileType).HasMaxLength(50);
            entity.Property(e => e.UploadDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Case).WithMany(p => p.Documents)
                .HasForeignKey(d => d.CaseId)
                .HasConstraintName("FK__Documents__CaseI__534D60F1");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.Documents)
                .HasForeignKey(d => d.UploadedBy)
                .HasConstraintName("FK_Documents_UploadedBy");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F11B872B213");

            entity.Property(e => e.Branch).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Position).HasMaxLength(100);
        });

        modelBuilder.Entity<FinancialReport>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__Financia__D5BD4805D844F4AD");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReportFile).HasMaxLength(300);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.FinancialReports)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Financial__Creat__797309D9");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoices__D796AAB58A19ED1E");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsPaid).HasDefaultValue(false);

            entity.HasOne(d => d.Client).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK__Invoices__Client__74AE54BC");
        });

        modelBuilder.Entity<Lawyer>(entity =>
        {
            entity.HasKey(e => e.LawyerId).HasName("PK__Lawyers__58AB6874266A4AB5");

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Specialty).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.Lawyers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Lawyers_Users");
        });

        modelBuilder.Entity<LegalDocument>(entity =>
        {
            entity.HasKey(e => e.LegalDocumentId).HasName("PK__LegalDoc__B09E0FA5A2660F5F");

            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.FilePath).HasMaxLength(300);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Messages__C87C0C9C4EA7B31B");

            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Receiver).WithMany(p => p.MessageReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .HasConstraintName("FK__Messages__Receiv__656C112C");

            entity.HasOne(d => d.Sender).WithMany(p => p.MessageSenders)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("FK__Messages__Sender__6477ECF3");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E12C8B9A11B");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.Message).HasMaxLength(300);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Type).HasMaxLength(100);

            entity.HasOne(d => d.RelatedCase).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.RelatedCaseId)
                .HasConstraintName("FK_Notifications_Cases");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__UserI__7D439ABD");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A7F679749");

            entity.Property(e => e.RoleName).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CAB2D03AC");

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(200);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        modelBuilder.Entity<Verdict>(entity =>
        {
            entity.HasKey(e => e.VerdictId).HasName("PK__Verdicts__EA0434CAFD09C29F");

            entity.Property(e => e.VerdictDate).HasColumnType("datetime");

            entity.HasOne(d => d.Case).WithMany(p => p.Verdicts)
                .HasForeignKey(d => d.CaseId)
                .HasConstraintName("FK__Verdicts__CaseId__6E01572D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
