using System;
using System.Collections.Generic;

namespace JournalApi.Models;

public partial class StudyDuration
{
    public int Id { get; set; }

    public string Period { get; set; } = null!;

    public int EducationTypeId { get; set; }

    public virtual EducationType EducationType { get; set; } = null!;

    public virtual ICollection<StudyDurationSpeciality> StudyDurationSpecialities { get; set; } = new List<StudyDurationSpeciality>();
}
