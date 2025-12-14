using System;
using System.Collections.Generic;

namespace JournalApi.Models;

public partial class Speciality
{
    public int Id { get; set; }

    public string Value { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<StudyDurationSpeciality> StudyDurationSpecialities { get; set; } = new List<StudyDurationSpeciality>();

    public virtual ICollection<SubjectSpecialty> SubjectSpecialties { get; set; } = new List<SubjectSpecialty>();
}
