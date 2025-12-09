using System;
using System.Collections.Generic;

namespace JournalApi.Models;

public partial class SubjectSpecialty
{
    public int Id { get; set; }

    public int SubjectId { get; set; }

    public int SpecialityId { get; set; }

    public virtual Speciality Speciality { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;
}
