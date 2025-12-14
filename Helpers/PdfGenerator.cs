using iTextSharp.text;
using iTextSharp.text.pdf;

namespace JournalApi.Helpers;

public static class PdfGenerator
{
    public static byte[] GenerateMonthlyReportPdf(string groupName, DateTime month, List<DTOs.Reports.StudentMonthlyReportDTO> students)
    {
        using var output = new MemoryStream();
        var document = new Document(PageSize.A4.Rotate());
        var writer = PdfWriter.GetInstance(document, output);
        document.Open();

        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
        var subtitleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
        var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

        document.Add(new Paragraph($"Отчёт по группе: {groupName}", titleFont));
        document.Add(new Paragraph($"Месяц: {month:MMMM yyyy}", subtitleFont));
        document.Add(new Paragraph(" "));

        var subjects = new HashSet<string>();
        foreach (var student in students)
        {
            foreach (var subjectGrades in student.SubjectGrades)
            {
                subjects.Add(subjectGrades.Key);
            }
        }
        var sortedSubjects = subjects.OrderBy(s => s).ToList();

        var table = new PdfPTable(2 + sortedSubjects.Count + 2);
        table.WidthPercentage = 100;

        var headerCell = new PdfPCell(new Phrase("ФИО Студента", subtitleFont)) { HorizontalAlignment = Element.ALIGN_CENTER };
        table.AddCell(headerCell);
        foreach (var subject in sortedSubjects)
        {
            var cell = new PdfPCell(new Phrase(subject, subtitleFont)) { HorizontalAlignment = Element.ALIGN_CENTER };
            table.AddCell(cell);
        }
        headerCell = new PdfPCell(new Phrase("Средний балл", subtitleFont)) { HorizontalAlignment = Element.ALIGN_CENTER };
        table.AddCell(headerCell);
        headerCell = new PdfPCell(new Phrase("Пропуски", subtitleFont)) { HorizontalAlignment = Element.ALIGN_CENTER };
        table.AddCell(headerCell);
        headerCell = new PdfPCell(new Phrase("Опоздания", subtitleFont)) { HorizontalAlignment = Element.ALIGN_CENTER };
        table.AddCell(headerCell);

        foreach (var student in students)
        {
            table.AddCell(new PdfPCell(new Phrase($"{student.Surname} {student.Name} {student.Patronymic}", cellFont)));

            for (int i = 0; i < sortedSubjects.Count; i++)
            {
                var subject = sortedSubjects[i];
                if (student.SubjectGrades.TryGetValue(subject, out var grades))
                {
                    table.AddCell(new PdfPCell(new Phrase(string.Join(", ", grades), cellFont)));
                }
                else
                {
                    table.AddCell(new PdfPCell(new Phrase("-", cellFont)));
                }
            }

            if (student.SubjectAverage.Count > 0)
            {
                var avg = student.SubjectAverage.Values.Average();
                table.AddCell(new PdfPCell(new Phrase(avg.ToString("F2"), cellFont)));
            }
            else
            {
                table.AddCell(new PdfPCell(new Phrase("-", cellFont)));
            }

            table.AddCell(new PdfPCell(new Phrase(student.TotalAbsences.ToString(), cellFont)));
            table.AddCell(new PdfPCell(new Phrase(student.TotalLateness.ToString(), cellFont)));
        }

        document.Add(table);
        document.Close();

        return output.ToArray();
    }

    public static byte[] GenerateSessionReportPdf(string groupName, int semesterNumber, int year, List<DTOs.Reports.StudentSessionReportDTO> students)
    {
        using var output = new MemoryStream();
        var document = new Document(PageSize.A4.Rotate());
        var writer = PdfWriter.GetInstance(document, output);
        document.Open();

        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
        var subtitleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
        var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

        document.Add(new Paragraph($"Отчёт по сессии: {groupName}", titleFont));
        document.Add(new Paragraph($"Семестр: {semesterNumber} ({year}-{year + 1})", subtitleFont));
        document.Add(new Paragraph(" "));

        var subjects = new HashSet<string>();
        foreach (var student in students)
        {
            foreach (var finalGrade in student.FinalGrades)
            {
                subjects.Add(finalGrade.Key);
            }
        }
        var sortedSubjects = subjects.OrderBy(s => s).ToList();

        var table = new PdfPTable(1 + sortedSubjects.Count + 1);
        table.WidthPercentage = 100;

        var headerCell = new PdfPCell(new Phrase("ФИО Студента", subtitleFont)) { HorizontalAlignment = Element.ALIGN_CENTER };
        table.AddCell(headerCell);
        foreach (var subject in sortedSubjects)
        {
            var cell = new PdfPCell(new Phrase(subject, subtitleFont)) { HorizontalAlignment = Element.ALIGN_CENTER };
            table.AddCell(cell);
        }
        headerCell = new PdfPCell(new Phrase("На академ. стипендии?", subtitleFont)) { HorizontalAlignment = Element.ALIGN_CENTER };
        table.AddCell(headerCell);

        foreach (var student in students)
        {
            table.AddCell(new PdfPCell(new Phrase($"{student.Surname} {student.Name} {student.Patronymic}", cellFont)));

            for (int i = 0; i < sortedSubjects.Count; i++)
            {
                var subject = sortedSubjects[i];
                student.FinalGrades.TryGetValue(subject, out var grade);
                table.AddCell(new PdfPCell(new Phrase(grade ?? "Нет оценки", cellFont)));
            }

            table.AddCell(new PdfPCell(new Phrase(student.IsAcademicProbation ? "Да" : "Нет", cellFont)));
        }

        document.Add(table);
        document.Close();

        return output.ToArray();
    }
}