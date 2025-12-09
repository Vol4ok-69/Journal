using System;
using System.Collections.Generic;

namespace JournalApi.Models;

public partial class LessonType
{
    public int Id { get; set; }

    public string Value { get; set; } = null!;

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
