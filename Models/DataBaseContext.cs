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

    public virtual DbSet<EducationType> EducationTypes { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<LessonType> LessonTypes { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<EmployeePost> Posts { get; set; }

    public virtual DbSet<Speciality> Specialities { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentGrade> StudentGrades { get; set; }

    public virtual DbSet<StudyDuration> StudyDurations { get; set; }

    public virtual DbSet<StudyDurationSpeciality> StudyDurationSpecialities { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<SubjectEmployee> SubjectEmployees { get; set; }

    public virtual DbSet<SubjectSpecialty> SubjectSpecialties { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(WebApplication.CreateBuilder().Configuration.GetConnectionString("Default"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Grades_pkey");

            entity.Property(e => e.Value)
                .HasMaxLength(10)
                .HasColumnName("Grade");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Groups_pkey");

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

        modelBuilder.Entity<EmployeePost>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Posts_pkey");

            entity.Property(e => e.Post)
                .HasMaxLength(50)
                .HasColumnName("Post");
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
