using JournalApi.DTOs.Grades;
using OfficeOpenXml;

namespace JournalApi.Helpers;

public static class ExcelExporter
{
    [Obsolete]
    static ExcelExporter()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public static byte[] ExportMonthlyReportToExcel(string groupName, DateTime month, List<DTOs.Reports.StudentMonthlyReportDTO> students)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Monthly Report");

        worksheet.Cells[1, 1].Value = "Группа:";
        worksheet.Cells[1, 2].Value = groupName;
        worksheet.Cells[2, 1].Value = "Месяц:";
        worksheet.Cells[2, 2].Value = month.ToString("MMMM yyyy");

        var currentRow = 4;
        worksheet.Cells[currentRow, 1].Value = "ФИО Студента";
        var subjects = new HashSet<string>();
        foreach (var student in students)
        {
            foreach (var subjectGrades in student.SubjectGrades)
            {
                subjects.Add(subjectGrades.Key);
            }
        }
        var sortedSubjects = subjects.OrderBy(s => s).ToList();
        for (int i = 0; i < sortedSubjects.Count; i++)
        {
            worksheet.Cells[currentRow, 2 + i].Value = sortedSubjects[i];
        }
        worksheet.Cells[currentRow, 2 + sortedSubjects.Count].Value = "Средний балл";
        worksheet.Cells[currentRow, 3 + sortedSubjects.Count].Value = "Пропуски";
        worksheet.Cells[currentRow, 4 + sortedSubjects.Count].Value = "Опоздания";

        currentRow++;

        foreach (var student in students)
        {
            worksheet.Cells[currentRow, 1].Value = $"{student.Surname} {student.Name} {student.Patronymic}";

            for (int i = 0; i < sortedSubjects.Count; i++)
            {
                var subject = sortedSubjects[i];
                if (student.SubjectGrades.TryGetValue(subject, out var grades))
                {
                    worksheet.Cells[currentRow, 2 + i].Value = string.Join(", ", grades);
                }
            }

            if (student.SubjectAverage.Count > 0)
            {
                var avg = student.SubjectAverage.Values.Average();
                worksheet.Cells[currentRow, 2 + sortedSubjects.Count].Value = avg;
            }

            worksheet.Cells[currentRow, 3 + sortedSubjects.Count].Value = student.TotalAbsences;
            worksheet.Cells[currentRow, 4 + sortedSubjects.Count].Value = student.TotalLateness;

            currentRow++;
        }

        return package.GetAsByteArray();
    }

    public static byte[] ExportSessionReportToExcel(string groupName, int semesterNumber, int year, List<DTOs.Reports.StudentSessionReportDTO> students)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Session Report");

        worksheet.Cells[1, 1].Value = "Группа:";
        worksheet.Cells[1, 2].Value = groupName;
        worksheet.Cells[2, 1].Value = "Семестр:";
        worksheet.Cells[2, 2].Value = $"{semesterNumber} ({year}-{year + 1})";

        var currentRow = 4;
        worksheet.Cells[currentRow, 1].Value = "ФИО Студента";
        var subjects = new HashSet<string>();
        foreach (var student in students)
        {
            foreach (var finalGrade in student.FinalGrades)
            {
                subjects.Add(finalGrade.Key);
            }
        }
        var sortedSubjects = subjects.OrderBy(s => s).ToList();
        for (int i = 0; i < sortedSubjects.Count; i++)
        {
            worksheet.Cells[currentRow, 2 + i].Value = sortedSubjects[i];
        }
        worksheet.Cells[currentRow, 2 + sortedSubjects.Count].Value = "На академ. стипендии?";

        currentRow++;

        foreach (var student in students)
        {
            worksheet.Cells[currentRow, 1].Value = $"{student.Surname} {student.Name} {student.Patronymic}";

            for (int i = 0; i < sortedSubjects.Count; i++)
            {
                var subject = sortedSubjects[i];
                student.FinalGrades.TryGetValue(subject, out var grade);
                worksheet.Cells[currentRow, 2 + i].Value = grade ?? "Нет оценки";
            }

            worksheet.Cells[currentRow, 2 + sortedSubjects.Count].Value = student.IsAcademicProbation ? "Да" : "Нет";

            currentRow++;
        }

        return package.GetAsByteArray();
    }
    public static byte[] ExportMonthlyGradesReportToExcel(int subjectId, DateTime month, List<StudentGradeDTO> studentGrades)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Monthly Grades Report");

        worksheet.Cells[1, 1].Value = "Subject ID:";
        worksheet.Cells[1, 2].Value = subjectId;
        worksheet.Cells[2, 1].Value = "Month:";
        worksheet.Cells[2, 2].Value = month.ToString("MMMM yyyy");

        var currentRow = 4;
        worksheet.Cells[currentRow, 1].Value = "ФИО Студента";
        worksheet.Cells[currentRow, 2].Value = "Оценки за месяц";
        worksheet.Cells[currentRow, 3].Value = "Комментарий";

        currentRow++;

        foreach (var studentGrade in studentGrades)
        {
            worksheet.Cells[currentRow, 1].Value = $"{studentGrade.StudentSurname} {studentGrade.StudentName} {studentGrade.StudentPatronymic}";
            worksheet.Cells[currentRow, 2].Value = string.Join(", ", studentGrade.Grades);
            worksheet.Cells[currentRow, 3].Value = studentGrade.Description;

            currentRow++;
        }

        return package.GetAsByteArray();
    }
}