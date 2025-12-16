using JournalApi.DTOs.Users;

namespace JournalApi.Services;

public class UserService(DataBaseContext context)
{
    private readonly DataBaseContext _context = context;

    public async Task<StudentProfileDTO?> GetStudentProfileAsync(int userId)
    {
        var student = await _context.Students
            .Include(s => s.Group)
                .ThenInclude(g => g.Speciality)
            .FirstOrDefaultAsync(s => s.Id == userId);

        if (student == null) return null;

        return new StudentProfileDTO
        {
            Id = student.Id,
            Surname = student.Surname,
            Name = student.Name,
            Patronymic = student.Patronymic,
            Birthday = student.Birthday,
            Login = student.Login ?? "",
            Phone = student.Phone,
            GroupCode = student.Group.Code,
            Speciality = student.Group.Speciality.Value
        };
    }

    public async Task<TeacherProfileDTO?> GetTeacherProfileAsync(int userId)
    {
        var employee = await _context.Employees
            .Include(e => e.Post)
            .FirstOrDefaultAsync(e => e.Id == userId);

        if (employee == null || !IsTeacher(employee.Post.Post)) return null;

        return new TeacherProfileDTO
        {
            Id = employee.Id,
            Surname = employee.Surname,
            Name = employee.Name,
            Patronymic = employee.Patronymic,
            Birthday = employee.Birthday,
            Login = employee.Login,
            Phone = employee.Phone,
            Post = employee.Post.Post,
            Salary = employee.Salary
        };
    }

    public async Task<CuratorProfileDTO?> GetCuratorProfileAsync(int userId)
    {
        var employee = await _context.Employees
            .Include(e => e.Post)
            .Include(e => e.Groups)
            .FirstOrDefaultAsync(e => e.Id == userId);

        if (employee == null || !IsCurator(employee.Post.Post)) return null;

        var group = employee.Groups.FirstOrDefault();

        return new CuratorProfileDTO
        {
            Id = employee.Id,
            Surname = employee.Surname,
            Name = employee.Name,
            Patronymic = employee.Patronymic,
            Birthday = employee.Birthday,
            Login = employee.Login,
            Phone = employee.Phone,
            Post = employee.Post.Post,
            Salary = employee.Salary,
            GroupCode = group?.Code ?? "Нет группы"
        };
    }

    public async Task<AdminProfileDTO?> GetAdminProfileAsync(int userId)
    {
        var employee = await _context.Employees
            .Include(e => e.Post)
            .FirstOrDefaultAsync(e => e.Id == userId);

        if (employee == null || !IsAdmin(employee.Post.Post)) return null;

        return new AdminProfileDTO
        {
            Id = employee.Id,
            Surname = employee.Surname,
            Name = employee.Name,
            Patronymic = employee.Patronymic,
            Birthday = employee.Birthday,
            Login = employee.Login,
            Phone = employee.Phone,
            Post = employee.Post.Post
        };
    }

    public async Task<object?> GetProfileAsync(int userId, string role)
    {
        return role.ToLower() switch
        {
            "student" => await GetStudentProfileAsync(userId),
            "teacher" => await GetTeacherProfileAsync(userId),
            "curator" => await GetCuratorProfileAsync(userId),
            "admin" => await GetAdminProfileAsync(userId),
            _ => null
        };
    }

    private static bool IsTeacher(string post) => post.Contains("Преподаватель", StringComparison.OrdinalIgnoreCase);
    private static bool IsCurator(string post) => post.Contains("Куратор", StringComparison.OrdinalIgnoreCase);
    private static bool IsAdmin(string post) => post.Contains("Администратор", StringComparison.OrdinalIgnoreCase);
}