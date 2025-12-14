namespace JournalApi.DTOs.Auth;

public class RegisterRequestDTO
{
    public string Surname { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Patronymic { get; set; }
    public DateOnly Birthday { get; set; }
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Role { get; set; } // e.g., "Student", "Teacher", "Curator", "Admin". Уточните, как роль передается при регистрации.
    // Для Student:
    public int? GroupId { get; set; } // Обязательно для студентов
    // Для Teacher/Curator/Admin:
    public int? PostId { get; set; } // Обязательно для сотрудников
    public decimal? Salary { get; set; } // Для сотрудников
}