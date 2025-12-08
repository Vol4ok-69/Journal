using System;
using System.Collections.Generic;

namespace JournalApi.Models;

public partial class StudyDurationSpeciality
{
    public int Id { get; set; }

    public int StudyDurationId { get; set; }

    public int SpecialityId { get; set; }

    public virtual Speciality Speciality { get; set; } = null!;

    public virtual StudyDuration StudyDuration { get; set; } = null!;
}
