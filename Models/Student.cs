using System;
using System.Collections.Generic;

namespace JournalApi.Models;

public partial class Student
{
    public int Id { get; set; }

    public string Surname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Patronymic { get; set; }

    public DateOnly Birthday { get; set; }

    public string? Login { get; set; }

    public string? Password { get; set; }

    public int GroupId { get; set; }

    public string? Phone { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual Group Group { get; set; } = null!;

    public virtual ICollection<StudentGrade> StudentGrades { get; set; } = new List<StudentGrade>();
}
