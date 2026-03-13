using JournalApi.DTOs.Reports;
using JournalApi.DTOs.Grades;
using JournalApi.Helpers;

namespace JournalApi.Services;

public class ReportService
{
    private readonly DataBaseContext _context;
    private readonly AttendanceService _attendanceService;
    private readonly GradeService _gradeService;

    public ReportService(DataBaseContext context, AttendanceService attendanceService, GradeService gradeService)
    {
        _context = context;
        _attendanceService = attendanceService;
        _gradeService = gradeService;
    }

    public async Task<MonthlyReportDTO?> GetMonthlyAttendanceReportAsync(int groupId, DateTime month)
    {
        var group = await _context.Groups
            .Include(g => g.Students)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group == null) return null;

        var startDate = new DateTime(month.Year, month.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var studentsMonthlyReport = new List<StudentMonthlyReportDTO>();

        foreach (var student in group.Students)
        {
            var attendanceForPeriod = await _attendanceService.GetAttendanceReportAsync(
                groupId: groupId,
                subjectId: null,
                studentId: student.Id,
                startDate: startDate,
                endDate: endDate
            );

            var totalAbsences = attendanceForPeriod.Count(a => !a.IsPresent);
            var totalLateness = attendanceForPeriod.Count(a => a.IsLate);

            studentsMonthlyReport.Add(new StudentMonthlyReportDTO
            {
                Surname = student.Surname,
                Name = student.Name,
                Patronymic = student.Patronymic ?? "",
                SubjectGrades = new Dictionary<string, List<string>>(),
                SubjectAverage = new Dictionary<string, double>(),
                TotalAbsences = totalAbsences,
                TotalLateness = totalLateness
            });
        }

        return new MonthlyReportDTO
        {
            Group = group.Code,
            Month = month,
            Students = studentsMonthlyReport
        };
    }

    public async Task<MonthlyGradeDTO?> GetMonthlyGradesReportAsync(int subjectId, DateTime month)
    {
        var startDate = new DateTime(month.Year, month.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var lessonsInPeriod = await _context.Lessons
            .Include(l => l.Group)
                .ThenInclude(g => g.Students)
            .Where(l => l.SubjectId == subjectId && l.Date >= startDate && l.Date <= endDate)
            .ToListAsync();

        if (!lessonsInPeriod.Any()) return null;

        var studentGradesDict = new Dictionary<int, StudentGradeDTO>();

        foreach (var lesson in lessonsInPeriod)
        {
            var lessonGrades = await _gradeService.GetGradesForLessonAsync(lesson.Id);

            foreach (var grade in lessonGrades)
            {
                if (!studentGradesDict.ContainsKey(grade.StudentId))
                {
                    studentGradesDict[grade.StudentId] = new StudentGradeDTO
                    {
                        StudentId = grade.StudentId,
                        StudentSurname = grade.StudentSurname,
                        StudentName = grade.StudentName,
                        StudentPatronymic = grade.StudentPatronymic,
                        Subject = grade.Subject,
                        Grades = new List<string>(),
                        Description = grade.Description
                    };
                }
                studentGradesDict[grade.StudentId].Grades.Add(grade.Grade);
            }
        }

        var monthlyGradeDTO = new MonthlyGradeDTO
        {
            SubjectId = subjectId,
            Month = month,
            StudentGrades = studentGradesDict.Values.ToList()
        };

        return monthlyGradeDTO;
    }

    public async Task<SessionReportDTO?> GetSessionReportAsync(int groupId, int semesterNumber, int year)
    {
        var group = await _context.Groups
            .Include(g => g.Students)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group == null) return null;


        DateTime startDate, endDate;
        if (semesterNumber == 1)
        {
            startDate = new DateTime(year, 9, 1);
            endDate = new DateTime(year, 12, 31);
        }
        else if (semesterNumber == 2)
        {
            startDate = new DateTime(year, 2, 1);
            endDate = new DateTime(year, 6, 31);
        }
        else
        {
            return null;
        }

        var studentsSessionReport = new List<StudentSessionReportDTO>();

        foreach (var student in group.Students)
        {
            var studentGradesForSemester = await _context.StudentGrades
                .Include(sg => sg.Lesson)
                    .ThenInclude(l => l.Subject)
                .Include(sg => sg.Grade)
                .Where(sg => sg.StudentId == student.Id && sg.Date >= startDate && sg.Date <= endDate)
                .ToListAsync();

            var finalGrades = new Dictionary<string, string>();
            var subjectGrades = studentGradesForSemester
                .GroupBy(sg => sg.Lesson.Subject.Value)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(sg => sg.Grade.Value).ToList()
                );

            foreach (var subjectEntry in subjectGrades)
            {
                var average = subjectEntry.Value.Select(v => ParseGradeValue(v)).Where(v => v.HasValue).Select(v => v.Value).DefaultIfEmpty(0).Average();
                var finalGrade = ConvertAverageToFinalGrade(average);
                finalGrades[subjectEntry.Key] = finalGrade;
            }

            bool hasAcademicProbation = true;
            foreach (var gradeEntry in studentGradesForSemester)
            {
                var gradeValue = gradeEntry.Grade.Value;
                var description = gradeEntry.Description?.ToLower().Replace('ё', 'е').Trim();

                if (description != null && (description.Contains("дифференциальный зачет") || description.Contains("экзамен")))
                {
                    if (gradeValue == "2" || gradeValue == "3")
                    {
                        hasAcademicProbation = false;
                        break;
                    }
                }
            }

            studentsSessionReport.Add(new StudentSessionReportDTO
            {
                Surname = student.Surname,
                Name = student.Name,
                Patronymic = student.Patronymic ?? "",
                FinalGrades = finalGrades,
                IsAcademicProbation = hasAcademicProbation
            });
        }

        return new SessionReportDTO
        {
            Group = group.Code,
            SemesterNumber = semesterNumber,
            Year = year,
            Students = studentsSessionReport
        };
    }

    private static string ConvertAverageToFinalGrade(double average)
    {
        if (average >= 4.65) return "5";
        if (average >= 3.65) return "4";
        if (average >= 2.65) return "3";
        if (average >= 2.0) return "2";
        if (average >= 0.0) return "Незачет";
        return "Нет данных";
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

    public async Task<Dictionary<string, object>> GetGradeStatisticsAsync(int? subjectId, int? groupId, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.StudentGrades
            .Include(sg => sg.Lesson)
                .ThenInclude(l => l.Subject)
            .Include(sg => sg.Student)
                .ThenInclude(s => s.Group)
            .Include(sg => sg.Grade);

        if (subjectId.HasValue)
        {
            query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<StudentGrade, Grade>)query.Where(sg => sg.Lesson.SubjectId == subjectId.Value);
        }
        if (groupId.HasValue)
        {
            query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<StudentGrade, Grade>)query.Where(sg => sg.Student.GroupId == groupId.Value);
        }
        if (startDate.HasValue)
        {
            query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<StudentGrade, Grade>)query.Where(sg => sg.Date >= startDate.Value);
        }
        if (endDate.HasValue)
        {
            query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<StudentGrade, Grade>)query.Where(sg => sg.Date <= endDate.Value);
        }

        var grades = await query
            .Select(sg => sg.Grade.Value)
            .ToListAsync();

        var gradeStats = grades
            .GroupBy(g => g)
            .ToDictionary(g => g.Key, g => g.Count());

        var totalGrades = grades.Count;
        var averageGrade = grades.Any() ? grades.Select(g => ParseGradeValue(g)).Where(v => v.HasValue).Select(v => v.Value).DefaultIfEmpty(0).Average() : 0.0;

        return new Dictionary<string, object>
        {
            { "GradeDistribution", gradeStats },
            { "TotalGrades", totalGrades },
            { "AverageGrade", averageGrade }
        };
    }

    public async Task<byte[]> ExportMonthlyAttendanceReportToExcelAsync(int groupId, DateTime month)
    {
        var report = await GetMonthlyAttendanceReportAsync(groupId, month) ?? throw new ArgumentException("Report not found");
        return ExcelExporter.ExportMonthlyReportToExcel(report.Group, report.Month, report.Students);
    }

    public async Task<byte[]> ExportMonthlyAttendanceReportToPdfAsync(int groupId, DateTime month)
    {
        var report = await GetMonthlyAttendanceReportAsync(groupId, month) ?? throw new ArgumentException("Report not found");
        return PdfGenerator.GenerateMonthlyReportPdf(report.Group, report.Month, report.Students);
    }

    public async Task<byte[]> ExportMonthlyGradesReportToExcelAsync(int subjectId, DateTime month)
    {
        var report = await GetMonthlyGradesReportAsync(subjectId, month) ?? throw new ArgumentException("Report not found");
        return ExcelExporter.ExportMonthlyGradesReportToExcel(report.SubjectId, report.Month, report.StudentGrades);
    }

    public async Task<byte[]> ExportMonthlyGradesReportToPdfAsync(int subjectId, DateTime month)
    {
        var report = await GetMonthlyGradesReportAsync(subjectId, month) ?? throw new ArgumentException("Report not found");
        return PdfGenerator.GenerateMonthlyGradesReportPdf(report.SubjectId, report.Month, report.StudentGrades);
    }

    public async Task<byte[]> ExportSessionReportToExcelAsync(int groupId, int semesterNumber, int year)
    {
        var report = await GetSessionReportAsync(groupId, semesterNumber, year) ?? throw new ArgumentException("Report not found");
        return ExcelExporter.ExportSessionReportToExcel(report.Group, semesterNumber, year, report.Students);
    }

    public async Task<byte[]> ExportSessionReportToPdfAsync(int groupId, int semesterNumber, int year)
    {
        var report = await GetSessionReportAsync(groupId, semesterNumber, year) ?? throw new ArgumentException("Report not found");
        return PdfGenerator.GenerateSessionReportPdf(report.Group, semesterNumber, year, report.Students);
    }
}