using JournalApi.DTOs.Grades;
using JournalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace JournalApi.Services;

public class GradeService(DataBaseContext context)
{
    private readonly DataBaseContext _context = context;

    public async Task<bool> AddGradesAsync(int lessonId, List<CreateGradeDTO> gradesDto)
    {
        var lesson = await _context.Lessons
            .Include(l => l.Group)
                .ThenInclude(g => g.Students)
            .FirstOrDefaultAsync(l => l.Id == lessonId);

        if (lesson == null) return false;

        var gradesToAdd = new List<StudentGrade>();

        foreach (var gradeDto in gradesDto)
        {
            var studentExistsInGroup = lesson.Group.Students.Any(s => s.Id == gradeDto.StudentId);

            if (!studentExistsInGroup) continue;

            var grade = new StudentGrade
            {
                StudentId = gradeDto.StudentId,
                LessonId = lessonId,
                GradeId = gradeDto.GradeId,
                Date = lesson.Date,
                Description = gradeDto.Description
            };

            gradesToAdd.Add(grade);
        }

        _context.StudentGrades.AddRange(gradesToAdd);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<StudentGradeDTO>> GetGradesForLessonAsync(int lessonId)
    {
        var lesson = await _context.Lessons
            .Include(l => l.Subject)
            .Include(l => l.Group)
                .ThenInclude(g => g.Students)
            .Include(l => l.StudentGrades)
                .ThenInclude(sg => sg.Student)
            .Include(l => l.StudentGrades)
                .ThenInclude(sg => sg.Grade)
            .FirstOrDefaultAsync(l => l.Id == lessonId);

        if (lesson == null) return new List<StudentGradeDTO>();

        var grades = new List<StudentGradeDTO>();

        foreach (var student in lesson.Group.Students)
        {
            var studentGradeEntry = lesson.StudentGrades.FirstOrDefault(sg => sg.StudentId == student.Id);

            grades.Add(new StudentGradeDTO
            {
                StudentId = student.Id,
                StudentSurname = student.Surname,
                StudentName = student.Name,
                StudentPatronymic = student.Patronymic ?? "",
                LessonId = lessonId,
                Subject = lesson.Subject.Value,
                Grade = studentGradeEntry?.Grade.Value ?? "Нет оценки",
                Description = studentGradeEntry?.Description
            });
        }

        return grades;
    }

    public async Task<List<StudentGradeDTO>> GetGradesForStudentBySubjectAsync(int studentId, int subjectId, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.StudentGrades
            .Include(sg => sg.Lesson)
                .ThenInclude(l => l.Subject)
            .Include(sg => sg.Student)
            .Include(sg => sg.Grade)
            .Where(sg => sg.StudentId == studentId && sg.Lesson.SubjectId == subjectId);

        if (startDate.HasValue)
        {
            query = query.Where(sg => sg.Date >= startDate.Value);
        }
        if (endDate.HasValue)
        {
            query = query.Where(sg => sg.Date <= endDate.Value);
        }

        var studentGrades = await query
            .OrderBy(sg => sg.Date)
            .ToListAsync();

        return [.. studentGrades.Select(sg => new StudentGradeDTO
        {
            StudentId = sg.StudentId,
            StudentSurname = sg.Student.Surname,
            StudentName = sg.Student.Name,
            StudentPatronymic = sg.Student.Patronymic ?? "",
            LessonId = sg.LessonId,
            Subject = sg.Lesson.Subject.Value,
            Grade = sg.Grade.Value,
            Description = sg.Description
        })];
    }

    public async Task<Dictionary<string, double>> GetAverageGradesForStudentAsync(int studentId, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.StudentGrades
            .Include(sg => sg.Lesson)
                .ThenInclude(l => l.Subject)
            .Include(sg => sg.Grade)
            .Where(sg => sg.StudentId == studentId);

        if (startDate.HasValue)
        {
            query = query.Where(sg => sg.Date >= startDate.Value);
        }
        if (endDate.HasValue)
        {
            query = query.Where(sg => sg.Date <= endDate.Value);
        }

        var gradeData = await query
            .Select(sg => new
            {
                SubjectName = sg.Lesson.Subject.Value,
                GradeValue = sg.Grade.Value
            })
            .ToListAsync();

        var subjectGrades = gradeData
            .GroupBy(x => x.SubjectName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => ParseGradeValue(x.GradeValue)).Where(v => v.HasValue).Select(v => v.Value).DefaultIfEmpty(0).Average()
            );

        return subjectGrades;
    }

    private static double? ParseGradeValue(string grade)
    {
        return grade switch
        {
            "5" => 5.0,
            "4" => 4.0,
            "3" => 3.0,
            "2" => 2.0,
            "Зачет" => 5.0,
            "Незачет" => 2.0,
            _ => null
        };
    }

    public async Task<List<StudentGradeDTO>> GetGradeReportAsync(int? groupId, int? subjectId, int? studentId, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.StudentGrades
            .Include(sg => sg.Lesson)
                .ThenInclude(l => l.Subject)
            .Include(sg => sg.Student)
                .ThenInclude(s => s.Group)
            .Include(sg => sg.Grade);

        if (groupId.HasValue)
        {
            query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<StudentGrade, Grade>)query.Where(sg => sg.Student.GroupId == groupId.Value);
        }
        if (subjectId.HasValue)
        {
            query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<StudentGrade, Grade>)query.Where(sg => sg.Lesson.SubjectId == subjectId.Value);
        }
        if (studentId.HasValue)
        {
            query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<StudentGrade, Grade>)query.Where(sg => sg.StudentId == studentId.Value);
        }
        if (startDate.HasValue)
        {
            query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<StudentGrade, Grade>)query.Where(sg => sg.Date >= startDate.Value);
        }
        if (endDate.HasValue)
        {
            query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<StudentGrade, Grade>)query.Where(sg => sg.Date <= endDate.Value);
        }

        var studentGrades = await query
            .OrderBy(sg => sg.Student.Surname)
            .ThenBy(sg => sg.Date)
            .ToListAsync();

        return [.. studentGrades.Select(sg => new StudentGradeDTO
        {
            StudentId = sg.StudentId,
            StudentSurname = sg.Student.Surname,
            StudentName = sg.Student.Name,
            StudentPatronymic = sg.Student.Patronymic ?? "",
            LessonId = sg.LessonId,
            Subject = sg.Lesson.Subject.Value,
            Grade = sg.Grade.Value,
            Description = sg.Description
        })];
    }

    public async Task<Dictionary<string, double>> GetAverageGradesForGroupAsync(int groupId, DateTime startDate, DateTime endDate)
    {
        var query = _context.StudentGrades
            .Include(sg => sg.Lesson)
                .ThenInclude(l => l.Subject)
            .Include(sg => sg.Grade)
            .Where(sg => sg.Student.GroupId == groupId && sg.Date >= startDate && sg.Date <= endDate);

        var gradeData = await query
            .Select(sg => new
            {
                SubjectName = sg.Lesson.Subject.Value,
                GradeValue = sg.Grade.Value
            })
            .ToListAsync();

        var subjectGrades = gradeData
            .GroupBy(x => x.SubjectName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => ParseGradeValue(x.GradeValue)).Where(v => v.HasValue).Select(v => v.Value).DefaultIfEmpty(0).Average()
            );

        return subjectGrades;
    }
}