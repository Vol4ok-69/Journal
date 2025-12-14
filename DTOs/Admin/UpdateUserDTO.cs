namespace JournalApi.DTOs.Admin;

public class UpdateUserDTO
{
    public string? Surname { get; set; }
    public string? Name { get; set; }
    public string? Patronymic { get; set; }
    public DateOnly? Birthday { get; set; }
    public string? Login { get; set; }
    public string? Phone { get; set; }
    // Для сотрудников:
    public decimal? Salary { get; set; }
    public int? PostId { get; set; }
    // Для студентов:
    public int? GroupId { get; set; }
}