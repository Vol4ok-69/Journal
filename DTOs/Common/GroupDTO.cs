namespace JournalApi.DTOs.Common;

public class GroupDTO
{
    public int Id { get; set; }
    public string Code { get; set; } = null!; // e.g., "ИСП-21"
    public int Course { get; set; }
    public int SpecialityId { get; set; }
    public int CuratorId { get; set; } // ID сотрудника-куратора
}