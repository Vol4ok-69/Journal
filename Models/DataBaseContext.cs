using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace JournalApi.Models;

public partial class DataBaseContext : DbContext
{
    public DataBaseContext()
    {
    }

    public DataBaseContext(DbContextOptions<DataBaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<EducationType> EducationTypes { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeePost> EmployeePosts { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<LessonType> LessonTypes { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Speciality> Specialities { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentGrade> StudentGrades { get; set; }

    public virtual DbSet<StudyDuration> StudyDurations { get; set; }

    public virtual DbSet<StudyDurationSpeciality> StudyDurationSpecialities { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<SubjectEmployee> SubjectEmployees { get; set; }

    public virtual DbSet<SubjectSpecialty> SubjectSpecialties { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=journal_db;Username=admin69;Password=admin69;Include Error Detail=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasIndex(e => e.LessonId, "IX_Attendances_LessonId");

            entity.HasIndex(e => e.StudentId, "IX_Attendances_StudentId");

            entity.HasOne(d => d.Lesson).WithMany(p => p.Attendances).HasForeignKey(d => d.LessonId);

            entity.HasOne(d => d.Student).WithMany(p => p.Attendances).HasForeignKey(d => d.StudentId);
        });

        modelBuilder.Entity<EducationType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("EducationTypes_pkey");

            entity.Property(e => e.Value).HasMaxLength(50);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Employees_pkey");

            entity.HasIndex(e => e.Login, "Employees_Login_key").IsUnique();

            entity.HasIndex(e => e.Phone, "Employees_Phone_key").IsUnique();

            entity.HasIndex(e => e.PostId, "IX_Employees_PostId");

            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(256);
            entity.Property(e => e.Patronymic).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Surname).HasMaxLength(50);

            entity.HasOne(d => d.Post).WithMany(p => p.Employees)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Employees_PostId_fkey");
        });

        modelBuilder.Entity<EmployeePost>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Posts_pkey");

            entity.Property(e => e.Post).HasMaxLength(50);
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Grades_pkey");

            entity.Property(e => e.Value).HasMaxLength(10);
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Groups_pkey");

            entity.HasIndex(e => e.CuratorId, "IX_Groups_CuratorId");

            entity.HasIndex(e => e.SpecialityId, "IX_Groups_SpecialityId");

            entity.Property(e => e.Code).HasMaxLength(20);

            entity.HasOne(d => d.Curator).WithMany(p => p.Groups)
                .HasForeignKey(d => d.CuratorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Groups_CuratorId_fkey");

            entity.HasOne(d => d.Speciality).WithMany(p => p.Groups)
                .HasForeignKey(d => d.SpecialityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Groups_SpecialityId_fkey");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Lessons_pkey");

            entity.HasIndex(e => e.GroupId, "IX_Lessons_GroupId");

            entity.HasIndex(e => e.LessonTypeId, "IX_Lessons_LessonTypeId");

            entity.HasIndex(e => e.SubjectId, "IX_Lessons_SubjectId");

            entity.HasIndex(e => e.TeacherId, "IX_Lessons_TeacherId");

            entity.Property(e => e.Date).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.Group).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Lessons_GroupId_fkey");

            entity.HasOne(d => d.LessonType).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.LessonTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Lessons_LessonTypeId_fkey");

            entity.HasOne(d => d.Subject).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Lessons_SubjectId_fkey");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Lessons_TeacherId_fkey");
        });

        modelBuilder.Entity<LessonType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("LessonTypes_pkey");

            entity.Property(e => e.Value).HasMaxLength(30);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.Property(e => e.Token).HasMaxLength(256);
        });

        modelBuilder.Entity<Speciality>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Specialities_pkey");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Value).HasMaxLength(100);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Students_pkey");

            entity.HasIndex(e => e.GroupId, "IX_Students_GroupId");

            entity.HasIndex(e => e.Login, "Students_Login_key").IsUnique();

            entity.HasIndex(e => e.Phone, "Students_Phone_key").IsUnique();

            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(256);
            entity.Property(e => e.Patronymic).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Surname).HasMaxLength(50);

            entity.HasOne(d => d.Group).WithMany(p => p.Students)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Students_GroupId_fkey");
        });

        modelBuilder.Entity<StudentGrade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("StudentGrades_pkey");

            entity.HasIndex(e => e.GradeId, "IX_StudentGrades_GradeId");

            entity.HasIndex(e => e.LessonId, "IX_StudentGrades_LessonId");

            entity.HasIndex(e => e.StudentId, "IX_StudentGrades_StudentId");

            entity.Property(e => e.Date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.Description).HasMaxLength(255);

            entity.HasOne(d => d.Grade).WithMany(p => p.StudentGrades)
                .HasForeignKey(d => d.GradeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("StudentGrades_GradeId_fkey");

            entity.HasOne(d => d.Lesson).WithMany(p => p.StudentGrades)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("StudentGrades_LessonId_fkey");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentGrades)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("StudentGrades_StudentId_fkey");
        });

        modelBuilder.Entity<StudyDuration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("StudyDurations_pkey");

            entity.HasIndex(e => e.EducationTypeId, "IX_StudyDurations_EducationTypeId");

            entity.Property(e => e.Period).HasMaxLength(50);

            entity.HasOne(d => d.EducationType).WithMany(p => p.StudyDurations)
                .HasForeignKey(d => d.EducationTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("StudyDurations_EducationTypeId_fkey");
        });

        modelBuilder.Entity<StudyDurationSpeciality>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("StudyDurationSpeciality_pkey");

            entity.ToTable("StudyDurationSpeciality");

            entity.HasIndex(e => e.SpecialityId, "IX_StudyDurationSpeciality_SpecialityId");

            entity.HasIndex(e => e.StudyDurationId, "IX_StudyDurationSpeciality_StudyDurationId");

            entity.HasOne(d => d.Speciality).WithMany(p => p.StudyDurationSpecialities)
                .HasForeignKey(d => d.SpecialityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("StudyDurationSpeciality_SpecialityId_fkey");

            entity.HasOne(d => d.StudyDuration).WithMany(p => p.StudyDurationSpecialities)
                .HasForeignKey(d => d.StudyDurationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("StudyDurationSpeciality_StudyDurationId_fkey");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Subjects_pkey");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Value).HasMaxLength(100);
        });

        modelBuilder.Entity<SubjectEmployee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SubjectEmployee_pkey");

            entity.ToTable("SubjectEmployee");

            entity.HasIndex(e => e.PossibleEmployeeId, "IX_SubjectEmployee_PossibleEmployeeId");

            entity.HasIndex(e => e.SubjectId, "IX_SubjectEmployee_SubjectId");

            entity.HasOne(d => d.PossibleEmployee).WithMany(p => p.SubjectEmployees)
                .HasForeignKey(d => d.PossibleEmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SubjectEmployee_PossibleEmployeeId_fkey");

            entity.HasOne(d => d.Subject).WithMany(p => p.SubjectEmployees)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SubjectEmployee_SubjectId_fkey");
        });

        modelBuilder.Entity<SubjectSpecialty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SubjectSpecialty_pkey");

            entity.ToTable("SubjectSpecialty");

            entity.HasIndex(e => e.SpecialityId, "IX_SubjectSpecialty_SpecialityId");

            entity.HasIndex(e => e.SubjectId, "IX_SubjectSpecialty_SubjectId");

            entity.HasOne(d => d.Speciality).WithMany(p => p.SubjectSpecialties)
                .HasForeignKey(d => d.SpecialityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SubjectSpecialty_SpecialityId_fkey");

            entity.HasOne(d => d.Subject).WithMany(p => p.SubjectSpecialties)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SubjectSpecialty_SubjectId_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
