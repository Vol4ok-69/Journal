using System;
using System.Collections.Generic;

namespace JournalApi.Models;

public partial class EducationType
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public virtual ICollection<StudyDuration> StudyDurations { get; set; } = new List<StudyDuration>();
}
