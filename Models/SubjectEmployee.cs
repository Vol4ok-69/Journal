using System;
using System.Collections.Generic;

namespace JournalApi.Models;

public partial class SubjectEmployee
{
    public int Id { get; set; }

    public int SubjectId { get; set; }

    public int PossibleEmployeeId { get; set; }

    public virtual Employee PossibleEmployee { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;
}
