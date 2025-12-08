using System;
using System.Collections.Generic;

namespace JournalApi.Models;

public partial class Group
{
    public int Id { get; set; }

    public int CuratorId { get; set; }

    public string Code { get; set; } = null!;

    public int Course { get; set; }

    public int SpecialityId { get; set; }

    public virtual Employee Curator { get; set; } = null!;

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public virtual Speciality Speciality { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
