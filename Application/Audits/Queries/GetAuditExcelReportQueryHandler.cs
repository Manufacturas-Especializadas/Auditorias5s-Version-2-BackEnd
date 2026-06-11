using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using Application.Audits.Queries;
using ClosedXML.Excel;
using MediatR;

namespace MesaCore.Audits.Application.Audits.Queries.GetAuditExcelReport;

public class GetAuditExcelReportQueryHandler : IRequestHandler<GetAuditExcelReportQuery, AuditExcelReportDto?>
{
    private readonly ApplicationDbContext _context;

    public GetAuditExcelReportQueryHandler(ApplicationDbContext context) => _context = context;

    public async Task<AuditExcelReportDto?> Handle(GetAuditExcelReportQuery request, CancellationToken cancellationToken)
    {
        var audit = await _context.Audits
        .Include(a => a.Auditor)
        .Include(a => a.Area)
            .ThenInclude(ar => ar.Module)
        .Include(a => a.Answers)
            .ThenInclude(ans => ans.Question)
                .ThenInclude(q => q.Category) 
        .FirstOrDefaultAsync(a => a.Id == request.AuditId, cancellationToken);

        if (audit == null) return null;

            return new AuditExcelReportDto(
            audit.Id,
            audit.Auditor!.FullName,
            audit.Area!.Name,
            audit.Area!.Module!.Name,
            audit.AuditDate,
            audit.FinalScore,
            audit.Answers.Select(ans => new AuditQuestionReportDto(
                ans.QuestionId,
                ans.Question!.Category?.Name ?? "General",
                ans.Question.QuestionText ?? "Sin descripción",
                ans.Score
            ))
            .OrderBy(x => x.CategoryTitle)
            .ThenBy(x => x.QuestionId)
            .ToList()
    );
    }

    public byte[] GenerateExcelByteArray(AuditExcelReportDto data)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Reporte 5S");

        var titleRange = worksheet.Range("A2:D2");
        titleRange.Merge().Value = "REPORTE DE AUDITORÍA DE CALIDAD 5S";

        titleRange.Style.Font.Bold = true;
        titleRange.Style.Font.FontSize = 16;
        titleRange.Style.Font.FontName = "Segoe UI";
        titleRange.Style.Font.FontColor = XLColor.White;
        titleRange.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#1E293B"));
        titleRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        titleRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        worksheet.Row(2).Height = 35;

        worksheet.Cell("A4").Value = "Módulo:";
        worksheet.Cell("B4").Value = data.ModuleName;
        worksheet.Cell("A5").Value = "Área / Línea:";
        worksheet.Cell("B5").Value = data.AreaName;

        worksheet.Cell("A7").Value = "Auditor:";
        worksheet.Cell("B7").Value = data.AuditorName;
        worksheet.Cell("A8").Value = "Fecha Registro:";
        worksheet.Cell("B8").Value = data.AuditDate.ToString("dd/MM/yyyy HH:mm:ss");

        var labelRange = worksheet.Range("A4:A8");
        labelRange.Style.Font.Bold = true;
        labelRange.Style.Font.FontColor = XLColor.FromHtml("#475569");
        labelRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

        var cardBackground = data.FinalScore >= 95M ? XLColor.FromHtml("#DCFCE7")
                           : data.FinalScore >= 85M ? XLColor.FromHtml("#E0F2FE")
                           : data.FinalScore >= 70M ? XLColor.FromHtml("#FEF3C7")
                           : XLColor.FromHtml("#FEE2E2");

        var cardTextColor = data.FinalScore >= 95M ? XLColor.FromHtml("#166534")
                          : data.FinalScore >= 85M ? XLColor.FromHtml("#075985")
                          : data.FinalScore >= 70M ? XLColor.FromHtml("#92400E")
                          : XLColor.FromHtml("#991B1B");

        var scoreLabel = worksheet.Cell("D4");
        scoreLabel.Value = "CALIFICACIÓN FINAL";
        scoreLabel.Style.Font.Bold = true;
        scoreLabel.Style.Font.FontSize = 9;
        scoreLabel.Style.Font.FontColor = XLColor.FromHtml("#64748B");
        scoreLabel.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        var scoreCell = worksheet.Cell("D5");
        scoreCell.Value = data.FinalScore / 100M;

        scoreCell.Style.NumberFormat.Format = "0.0";
        scoreCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        scoreCell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

        scoreCell.Style.Font.Bold = true;
        scoreCell.Style.Font.FontSize = 24;
        scoreCell.Style.Font.FontColor = cardTextColor;

        worksheet.Range("D5:D7").Merge();
        var cardRange = worksheet.Range("D4:D7");
        cardRange.Style.Fill.SetBackgroundColor(cardBackground);
        cardRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.FromHtml("#CBD5E1"));

        int startRow = 10;

        worksheet.Cell(startRow, 1).Value = "Categoría 5S";
        worksheet.Cell(startRow, 2).Value = "ID Pregunta";
        worksheet.Cell(startRow, 3).Value = "Aspecto Evaluado";
        worksheet.Cell(startRow, 4).Value = "Calificación (1-5)";

        var tableHeaderRange = worksheet.Range(startRow, 1, startRow, 4);
        tableHeaderRange.Style.Font.Bold = true;
        tableHeaderRange.Style.Font.FontColor = XLColor.White;
        tableHeaderRange.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#334155"));
        tableHeaderRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        tableHeaderRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        worksheet.Row(startRow).Height = 25;

        int currentRow = startRow + 1;
        foreach (var ans in data.Answers)
        {
            worksheet.Cell(currentRow, 1).Value = ans.CategoryTitle;
            worksheet.Cell(currentRow, 2).Value = ans.QuestionId;
            worksheet.Cell(currentRow, 3).Value = ans.QuestionText;
            worksheet.Cell(currentRow, 4).Value = ans.Score;

            worksheet.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(currentRow, 2).Style.Font.FontName = "Consolas";
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            worksheet.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(currentRow, 4).Style.Font.Bold = true;

            if (currentRow % 2 == 0)
            {
                worksheet.Range(currentRow, 1, currentRow, 4).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#F8FAFC"));
            }

            worksheet.Range(currentRow, 1, currentRow, 4).Style.Border.SetBottomBorder(XLBorderStyleValues.Thin).Border.SetBottomBorderColor(XLColor.FromHtml("#F1F5F9"));

            currentRow++;
        }

        worksheet.Columns(1, 4).AdjustToContents();

        worksheet.Column(3).Width = 70;
        worksheet.Column(3).Style.Alignment.SetWrapText(true);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}