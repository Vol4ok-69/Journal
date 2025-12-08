using System;
using System.Collections.Generic;

namespace JournalApi.Models;

public partial class StudentGrade
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int LessonId { get; set; }

    public int GradeId { get; set; }

    public DateTime Date { get; set; }

    public string? Description { get; set; }

    public virtual Grade Grade { get; set; } = null!;

    public virtual Lesson Lesson { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
