using EduProcessManager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EduProcessManager.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Test> Tests => Set<Test>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<AnswerOption> AnswerOptions => Set<AnswerOption>();
    public DbSet<TestAttempt> TestAttempts => Set<TestAttempt>();
    public DbSet<EduEvent> EduEvents => Set<EduEvent>();
    public DbSet<EventRegistration> EventRegistrations => Set<EventRegistration>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<StudentGroup> StudentGroups { get; set; }
    public DbSet<TestStudentGroup> TestStudentGroups { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<EventRegistration>()
            .HasIndex(x => new { x.EduEventId, x.StudentId })
            .IsUnique();

        builder.Entity<EventRegistration>()
            .HasOne(x => x.EduEvent)
            .WithMany(x => x.Registrations)
            .HasForeignKey(x => x.EduEventId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<EventRegistration>()
            .HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TestAttempt>()
            .HasOne(x => x.Test)
            .WithMany(x => x.Attempts)
            .HasForeignKey(x => x.TestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TestAttempt>()
            .HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TestStudentGroup>()
            .HasKey(x => new { x.TestId, x.StudentGroupId });

        builder.Entity<TestStudentGroup>()
            .HasOne(x => x.Test)
            .WithMany(x => x.TestStudentGroups)
            .HasForeignKey(x => x.TestId);

        builder.Entity<TestStudentGroup>()
            .HasOne(x => x.StudentGroup)
            .WithMany(x => x.TestStudentGroups)
            .HasForeignKey(x => x.StudentGroupId);

        builder.Entity<ApplicationUser>()
            .HasOne(x => x.StudentGroup)
            .WithMany(x => x.Students)
            .HasForeignKey(x => x.StudentGroupId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
