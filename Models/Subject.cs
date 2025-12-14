using System;
using System.Collections.Generic;

namespace JournalApi.Models;

public partial class Subject
{
    public int Id { get; set; }

    public string Value { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public virtual ICollection<SubjectEmployee> SubjectEmployees { get; set; } = new List<SubjectEmployee>();

    public virtual ICollection<SubjectSpecialty> SubjectSpecialties { get; set; } = new List<SubjectSpecialty>();
}
