using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();

    public DbSet<Mark> Marks => Set<Mark>();

    public DbSet<AcademicYear> AcademicYears => Set<AcademicYear>();

    public DbSet<Subject> Subjects => Set<Subject>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ---------- Students ----------
        modelBuilder.Entity<Student>(student =>
        {
            student.Property(s => s.Name)
                .HasMaxLength(Student.NameMaxLength)
                .IsRequired();

            // Exactly 14 characters, stored as plain digits.
            student.Property(s => s.NationalId)
                .HasMaxLength(Student.NationalIdLength)
                .IsFixedLength()
                .IsUnicode(false)
                .IsRequired();

            // No two students can share a National ID.
            student.HasIndex(s => s.NationalId).IsUnique();

            // Last line of defence: the database itself rejects anything that isn't 14 digits.
            student.ToTable(t => t.HasCheckConstraint(
                "CK_Students_NationalId",
                $"LEN([NationalId]) = {Student.NationalIdLength} AND [NationalId] NOT LIKE '%[^0-9]%'"));

            // An academic year that has students can't be deleted.
            student.HasOne(s => s.AcademicYear)
                .WithMany()
                .HasForeignKey(s => s.AcademicYearId)
                .OnDelete(DeleteBehavior.Restrict);

            // Deleting a student also deletes their marks.
            student.HasMany(s => s.Marks)
                .WithOne(m => m.Student)
                .HasForeignKey(m => m.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ---------- Marks ----------
        modelBuilder.Entity<Mark>(mark =>
        {
            // One row per student per subject.
            mark.HasIndex(m => new { m.StudentId, m.SubjectId }).IsUnique();

            // Each term is out of 30.
            mark.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Marks_Term1", $"[Term1] BETWEEN 0 AND {Mark.MaxTermMark}");
                t.HasCheckConstraint("CK_Marks_Term2", $"[Term2] BETWEEN 0 AND {Mark.MaxTermMark}");
            });

            // A subject that has marks can't be deleted.
            mark.HasOne(m => m.Subject)
                .WithMany()
                .HasForeignKey(m => m.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ---------- Academic years (seed data) ----------
        modelBuilder.Entity<AcademicYear>(year =>
        {
            year.Property(y => y.Name).HasMaxLength(50).IsRequired();

            year.HasData(
                new AcademicYear { Id = 1, Name = "1st Primary" },
                new AcademicYear { Id = 2, Name = "2nd Primary" },
                new AcademicYear { Id = 3, Name = "3rd Primary" },
                new AcademicYear { Id = 4, Name = "4th Primary" },
                new AcademicYear { Id = 5, Name = "5th Primary" },
                new AcademicYear { Id = 6, Name = "6th Primary" },
                new AcademicYear { Id = 7, Name = "1st Preparatory" },
                new AcademicYear { Id = 8, Name = "2nd Preparatory" },
                new AcademicYear { Id = 9, Name = "3rd Preparatory" },
                new AcademicYear { Id = 10, Name = "1st Secondary" },
                new AcademicYear { Id = 11, Name = "2nd Secondary" },
                new AcademicYear { Id = 12, Name = "3rd Secondary" });
        });

        // ---------- Subjects (seed data) ----------
        modelBuilder.Entity<Subject>(subject =>
        {
            subject.Property(s => s.Name).HasMaxLength(50).IsRequired();

            subject.HasData(
                new Subject { Id = 1, Name = "Arabic" },
                new Subject { Id = 2, Name = "English" },
                new Subject { Id = 3, Name = "Mathematics" },
                new Subject { Id = 4, Name = "Science" });
        });
    }
}
