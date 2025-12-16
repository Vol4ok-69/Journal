using JournalApi.DTOs.Lessons;

namespace JournalApi.Services;

public class LessonService(DataBaseContext context)
{
    private readonly DataBaseContext _context = context;

    public async Task<LessonDTO?> GetLessonByIdAsync(int lessonId)
    {
        var lesson = await _context.Lessons
            .Include(l => l.Subject)
            .Include(l => l.Teacher)
            .Include(l => l.Group)
            .Include(l => l.LessonType)
            .FirstOrDefaultAsync(l => l.Id == lessonId);

        if (lesson == null) return null;

        return new LessonDTO
        {
            Id = lesson.Id,
            Subject = lesson.Subject.Value,
            Teacher = $"{lesson.Teacher.Surname} {lesson.Teacher.Name} {lesson.Teacher.Patronymic ?? ""}",
            Date = lesson.Date,
            Group = lesson.Group.Code,
            Number = lesson.Number,
            LessonType = lesson.LessonType.Value,
            Topic = lesson.Topic
        };
    }

    public async Task<LessonDTO> CreateLessonAsync(CreateLessonDTO lessonDto)
    {
        var subject = await _context.Subjects.FindAsync(lessonDto.SubjectId);
        var teacher = await _context.Employees.FindAsync(lessonDto.TeacherId);
        var group = await _context.Groups.FindAsync(lessonDto.GroupId);
        var lessonType = await _context.LessonTypes.FindAsync(lessonDto.LessonTypeId);

        if (subject == null || teacher == null || group == null || lessonType == null)
        {
            throw new ArgumentException("One or more related entities not found.");
        }

        var lesson = new Lesson
        {
            SubjectId = lessonDto.SubjectId,
            TeacherId = lessonDto.TeacherId,
            Date = lessonDto.Date,
            GroupId = lessonDto.GroupId,
            Number = lessonDto.Number,
            LessonTypeId = lessonDto.LessonTypeId,
            Topic = lessonDto.Topic
        };

        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync();

        return await GetLessonByIdAsync(lesson.Id) ?? throw new InvalidOperationException("Lesson was created but could not be retrieved.");
    }

    public async Task<LessonDTO?> UpdateLessonAsync(int lessonId, UpdateLessonDTO lessonDto)
    {
        var lesson = await _context.Lessons.FindAsync(lessonId);
        if (lesson == null) return null;

        if (lessonDto.SubjectId != lesson.SubjectId)
        {
            var subject = await _context.Subjects.FindAsync(lessonDto.SubjectId);
            if (subject == null) throw new ArgumentException("Subject not found.");
        }
        if (lessonDto.TeacherId != lesson.TeacherId)
        {
            var teacher = await _context.Employees.FindAsync(lessonDto.TeacherId);
            if (teacher == null) throw new ArgumentException("Teacher not found.");
        }
        if (lessonDto.GroupId != lesson.GroupId)
        {
            var group = await _context.Groups.FindAsync(lessonDto.GroupId);
            if (group == null) throw new ArgumentException("Group not found.");
        }
        if (lessonDto.LessonTypeId != lesson.LessonTypeId)
        {
            var lessonType = await _context.LessonTypes.FindAsync(lessonDto.LessonTypeId);
            if (lessonType == null) throw new ArgumentException("LessonType not found.");
        }

        lesson.SubjectId = lessonDto.SubjectId;
        lesson.Date = lessonDto.Date;
        lesson.GroupId = lessonDto.GroupId;
        lesson.Number = lessonDto.Number;
        lesson.LessonTypeId = lessonDto.LessonTypeId;
        lesson.Topic = lessonDto.Topic;

        await _context.SaveChangesAsync();

        return await GetLessonByIdAsync(lessonId);
    }

    public async Task<bool> DeleteLessonAsync(int lessonId)
    {
        var lesson = await _context.Lessons.FindAsync(lessonId);
        if (lesson == null) return false;

        var studentGrades = _context.StudentGrades.Where(sg => sg.LessonId == lessonId);
        var attendances = _context.Attendances.Where(a => a.LessonId == lessonId);

        _context.StudentGrades.RemoveRange(studentGrades);
        _context.Attendances.RemoveRange(attendances);

        _context.Lessons.Remove(lesson);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<LessonDTO>> GetLessonsByGroupAndSubjectAsync(int groupId, int subjectId)
    {
        var lessons = await _context.Lessons
            .Include(l => l.Subject)
            .Include(l => l.Teacher)
            .Include(l => l.Group)
            .Include(l => l.LessonType)
            .Where(l => l.GroupId == groupId && l.SubjectId == subjectId)
            .OrderBy(l => l.Date)
            .ToListAsync();

        return [.. lessons.Select(l => new LessonDTO
        {
            Id = l.Id,
            Subject = l.Subject.Value,
            Teacher = $"{l.Teacher.Surname} {l.Teacher.Name} {l.Teacher.Patronymic ?? ""}",
            Date = l.Date,
            Group = l.Group.Code,
            Number = l.Number,
            LessonType = l.LessonType.Value,
            Topic = l.Topic
        })];
    }

    public async Task<List<LessonDTO>> GetLessonsByGroupAsync(int groupId, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Lessons
            .Include(l => l.Subject)
            .Include(l => l.Teacher)
            .Include(l => l.Group)
            .Include(l => l.LessonType)
            .Where(l => l.GroupId == groupId);

        if (startDate.HasValue)
        {
            query = query.Where(l => l.Date >= startDate.Value);
        }
        if (endDate.HasValue)
        {
            query = query.Where(l => l.Date <= endDate.Value);
        }

        var lessons = await query
            .OrderBy(l => l.Date)
            .ToListAsync();

        return [.. lessons.Select(l => new LessonDTO
        {
            Id = l.Id,
            Subject = l.Subject.Value,
            Teacher = $"{l.Teacher.Surname} {l.Teacher.Name} {l.Teacher.Patronymic ?? ""}",
            Date = l.Date,
            Group = l.Group.Code,
            Number = l.Number,
            LessonType = l.LessonType.Value,
            Topic = l.Topic
        })];
    }

    public async Task<List<LessonDTO>> GetLessonsBySubjectAsync(int subjectId, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Lessons
            .Include(l => l.Subject)
            .Include(l => l.Teacher)
            .Include(l => l.Group)
            .Include(l => l.LessonType)
            .Where(l => l.SubjectId == subjectId);

        if (startDate.HasValue)
        {
            query = query.Where(l => l.Date >= startDate.Value);
        }
        if (endDate.HasValue)
        {
            query = query.Where(l => l.Date <= endDate.Value);
        }

        var lessons = await query
            .OrderBy(l => l.Date)
            .ToListAsync();

        return [.. lessons.Select(l => new LessonDTO
        {
            Id = l.Id,
            Subject = l.Subject.Value,
            Teacher = $"{l.Teacher.Surname} {l.Teacher.Name} {l.Teacher.Patronymic ?? ""}",
            Date = l.Date,
            Group = l.Group.Code,
            Number = l.Number,
            LessonType = l.LessonType.Value,
            Topic = l.Topic
        })];
    }

    public async Task<List<LessonDTO>> GetLessonsByTeacherAsync(int teacherId, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Lessons
            .Include(l => l.Subject)
            .Include(l => l.Teacher)
            .Include(l => l.Group)
            .Include(l => l.LessonType)
            .Where(l => l.TeacherId == teacherId);

        if (startDate.HasValue)
        {
            query = query.Where(l => l.Date >= startDate.Value);
        }
        if (endDate.HasValue)
        {
            query = query.Where(l => l.Date <= endDate.Value);
        }

        var lessons = await query
            .OrderBy(l => l.Date)
            .ToListAsync();

        return [.. lessons.Select(l => new LessonDTO
        {
            Id = l.Id,
            Subject = l.Subject.Value,
            Teacher = $"{l.Teacher.Surname} {l.Teacher.Name} {l.Teacher.Patronymic ?? ""}",
            Date = l.Date,
            Group = l.Group.Code,
            Number = l.Number,
            LessonType = l.LessonType.Value,
            Topic = l.Topic
        })];
    }

    public async Task<List<Student>> GetStudentsForLessonAsync(int lessonId)
    {
        var lesson = await _context.Lessons
            .Include(l => l.Group)
                .ThenInclude(g => g.Students)
            .FirstOrDefaultAsync(l => l.Id == lessonId);

        if (lesson == null) return [];

        return [.. lesson.Group.Students];
    }
}