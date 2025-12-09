using System;
using System.Collections.Generic;

namespace JournalApi.Models;

public partial class Grade
{
    public int Id { get; set; }

    public string Value { get; set; } = null!;

    public virtual ICollection<StudentGrade> StudentGrades { get; set; } = new List<StudentGrade>();
}
