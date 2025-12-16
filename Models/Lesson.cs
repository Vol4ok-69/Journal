using System;
using System.Collections.Generic;

namespace JournalApi.Models;

public partial class Lesson
{
    public int Id { get; set; }

    public int SubjectId { get; set; }

    public int TeacherId { get; set; }

    public DateTime Date { get; set; }

    public int GroupId { get; set; }

    public int Number { get; set; }

    public int LessonTypeId { get; set; }

    public string? Topic { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual Group Group { get; set; } = null!;

    public virtual LessonType LessonType { get; set; } = null!;

    public virtual ICollection<StudentGrade> StudentGrades { get; set; } = new List<StudentGrade>();

    public virtual Subject Subject { get; set; } = null!;

    public virtual Employee Teacher { get; set; } = null!;
}
