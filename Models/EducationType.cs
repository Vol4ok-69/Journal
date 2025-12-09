namespace JournalApi.Models;

public partial class EducationType
{
    public int Id { get; set; }

    public string Value { get; set; } = null!;

    public virtual ICollection<StudyDuration> StudyDurations { get; set; } = new List<StudyDuration>();
}
