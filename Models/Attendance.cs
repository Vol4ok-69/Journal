using System;
using System.Collections.Generic;

namespace JournalApi.Models;

public partial class Attendance
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int LessonId { get; set; }

    public bool IsPresent { get; set; }

    public bool IsLate { get; set; }

    public string? Comment { get; set; }

    public virtual Lesson Lesson { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
