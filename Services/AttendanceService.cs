using JournalApi.DTOs.Attendance;

namespace JournalApi.Services;

public class AttendanceService(DataBaseContext context)
{
    private readonly DataBaseContext _context = context;

    public async Task<bool> MarkAttendanceAsync(int lessonId, List<MarkAttendanceDTO> attendanceList)
    {
        var lesson = await _context.Lessons.FindAsync(lessonId);
        if (lesson == null) return false;

        var existingAttendance = await _context.Attendances
            .Where(a => a.LessonId == lessonId)
            .ToListAsync();

        _context.Attendances.RemoveRange(existingAttendance);

        var attendanceRecords = new List<Attendance>();

        foreach (var attendanceDto in attendanceList)
        {
            var student = await _context.Students
                .Where(s => s.Id == attendanceDto.StudentId && s.GroupId == lesson.GroupId)
                .FirstOrDefaultAsync();

            if (student == null) continue;

            var attendance = new Attendance
            {
                StudentId = attendanceDto.StudentId,
                LessonId = lessonId,
                IsPresent = attendanceDto.IsPresent,
                IsLate = attendanceDto.IsLate,
                Comment = attendanceDto.Comment
            };

            attendanceRecords.Add(attendance);
        }

        _context.Attendances.AddRange(attendanceRecords);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<AttendanceDTO>> GetAttendanceForLessonAsync(int lessonId)
    {
        var lesson = await _context.Lessons
            .Include(l => l.Group)
                .ThenInclude(g => g.Students)
            .Include(l => l.Attendances)
                .ThenInclude(a => a.Student)
            .FirstOrDefaultAsync(l => l.Id == lessonId);

        if (lesson == null) return new List<AttendanceDTO>();

        var attendanceList = new List<AttendanceDTO>();

        foreach (var student in lesson.Group.Students)
        {
            var studentAttendance = lesson.Attendances.FirstOrDefault(a => a.StudentId == student.Id);

            attendanceList.Add(new AttendanceDTO
            {
                StudentId = student.Id,
                StudentSurname = student.Surname,
                StudentName = student.Name,
                StudentPatronymic = student.Patronymic ?? "",
                LessonId = lessonId,
                IsPresent = studentAttendance?.IsPresent ?? true,
                IsLate = studentAttendance?.IsLate ?? false,
                Comment = studentAttendance?.Comment
            });
        }

        return attendanceList;
    }

    public async Task<List<AttendanceDTO>> GetAttendanceForStudentBySubjectAsync(int studentId, int subjectId, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Attendances
            .Include(a => a.Lesson)
                .ThenInclude(l => l.Subject)
            .Include(a => a.Student)
            .Where(a => a.StudentId == studentId && a.Lesson.SubjectId == subjectId);

        if (startDate.HasValue)
        {
            query = query.Where(a => a.Lesson.Date >= startDate.Value);
        }
        if (endDate.HasValue)
        {
            query = query.Where(a => a.Lesson.Date <= endDate.Value);
        }

        var attendances = await query
            .OrderBy(a => a.Lesson.Date)
            .ToListAsync();

        return [.. attendances.Select(a => new AttendanceDTO
        {
            StudentId = a.StudentId,
            StudentSurname = a.Student.Surname,
            StudentName = a.Student.Name,
            StudentPatronymic = a.Student.Patronymic ?? "",
            LessonId = a.Lesson.Id,
            IsPresent = a.IsPresent,
            IsLate = a.IsLate,
            Comment = a.Comment
        })];
    }

    public async Task<List<AttendanceDTO>> GetAttendanceForGroupBySubjectAsync(int groupId, int subjectId, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Attendances
            .Include(a => a.Lesson)
                .ThenInclude(l => l.Subject)
            .Include(a => a.Student)
            .ThenInclude(s => s.Group)
            .Where(a => a.Student.GroupId == groupId && a.Lesson.SubjectId == subjectId);

        if (startDate.HasValue)
        {
            query = query.Where(a => a.Lesson.Date >= startDate.Value);
        }
        if (endDate.HasValue)
        {
            query = query.Where(a => a.Lesson.Date <= endDate.Value);
        }

        var attendances = await query
            .OrderBy(a => a.Student.Surname)
            .ThenBy(a => a.Lesson.Date)
            .ToListAsync();

        return [.. attendances.Select(a => new AttendanceDTO
        {
            StudentId = a.StudentId,
            StudentSurname = a.Student.Surname,
            StudentName = a.Student.Name,
            StudentPatronymic = a.Student.Patronymic ?? "",
            LessonId = a.Lesson.Id,
            IsPresent = a.IsPresent,
            IsLate = a.IsLate,
            Comment = a.Comment
        })];
    }

    public async Task<List<AttendanceDTO>> GetAttendanceReportAsync(int? groupId, int? subjectId, int? studentId, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Attendances
            .Include(a => a.Lesson)
                .ThenInclude(l => l.Subject)
            .Include(a => a.Student)
                .ThenInclude(s => s.Group);

        if (groupId.HasValue)
        {
            query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Attendance, Group>)query.Where(a => a.Student.GroupId == groupId.Value);
        }
        if (subjectId.HasValue)
        {
            query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Attendance, Group>)query.Where(a => a.Lesson.SubjectId == subjectId.Value);
        }
        if (studentId.HasValue)
        {
            query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Attendance, Group>)query.Where(a => a.StudentId == studentId.Value);
        }
        if (startDate.HasValue)
        {
            query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Attendance, Group>)query.Where(a => a.Lesson.Date >= startDate.Value);
        }
        if (endDate.HasValue)
        {
            query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Attendance, Group>)query.Where(a => a.Lesson.Date <= endDate.Value);
        }

        var attendances = await query
            .OrderBy(a => a.Student.Surname)
            .ThenBy(a => a.Lesson.Date)
            .ToListAsync();

        return [.. attendances.Select(a => new AttendanceDTO
        {
            StudentId = a.StudentId,
            StudentSurname = a.Student.Surname,
            StudentName = a.Student.Name,
            StudentPatronymic = a.Student.Patronymic ?? "",
            LessonId = a.Lesson.Id,
            IsPresent = a.IsPresent,
            IsLate = a.IsLate,
            Comment = a.Comment
        })];
    }

    public async Task<Dictionary<int, (int totalLessons, int absences, int lateness)>> GetAttendanceSummaryForGroupAsync(int groupId, DateTime startDate, DateTime endDate)
    {
        var summary = new Dictionary<int, (int totalLessons, int absences, int lateness)>();

        var lessons = await _context.Lessons
            .Where(l => l.GroupId == groupId && l.Date >= startDate && l.Date <= endDate)
            .ToListAsync();

        var lessonIds = lessons.Select(l => l.Id).ToList();

        var attendances = await _context.Attendances
            .Where(a => lessonIds.Contains(a.LessonId))
            .ToListAsync();

        var groupedAttendances = attendances.GroupBy(a => a.StudentId);

        foreach (var studentGroup in groupedAttendances)
        {
            var studentId = studentGroup.Key;
            var studentAttendances = studentGroup.ToList();
            var totalLessons = studentAttendances.Count;
            var absences = studentAttendances.Count(a => !a.IsPresent);
            var lateness = studentAttendances.Count(a => a.IsLate);

            summary[studentId] = (totalLessons, absences, lateness);
        }

        return summary;
    }
}