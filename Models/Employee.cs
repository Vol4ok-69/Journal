using System;
using System.Collections.Generic;

namespace JournalApi.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string Surname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Patronymic { get; set; }

    public DateOnly Birthday { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Phone { get; set; }

    public decimal Salary { get; set; }

    public int PostId { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public virtual EmployeePost Post { get; set; } = null!;

    public virtual ICollection<SubjectEmployee> SubjectEmployees { get; set; } = new List<SubjectEmployee>();
}
