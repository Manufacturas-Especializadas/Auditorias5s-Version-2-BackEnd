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

        decimal promedioGeneral100 = data.Answers.Count > 0
            ? (decimal)data.Answers.Average(ans => ans.Score) * 20M
            : 0M;

        var resumenPorS = data.Answers
            .GroupBy(ans => ans.CategoryTitle)
            .Select(g => new {
                Categoria = g.Key,
                Promedio100 = (decimal)g.Average(ans => ans.Score) * 20M
            })
            .OrderBy(g => g.Categoria)
            .ToList();

        var cardBackground = promedioGeneral100 >= 95M ? XLColor.FromHtml("#DCFCE7")
                           : promedioGeneral100 >= 85M ? XLColor.FromHtml("#E0F2FE")
                           : promedioGeneral100 >= 70M ? XLColor.FromHtml("#FEF3C7")
                           : XLColor.FromHtml("#FEE2E2");

        var cardTextColor = promedioGeneral100 >= 95M ? XLColor.FromHtml("#166534")
                          : promedioGeneral100 >= 85M ? XLColor.FromHtml("#075985")
                          : promedioGeneral100 >= 70M ? XLColor.FromHtml("#92400E")
                          : XLColor.FromHtml("#991B1B");

        var wsDetalle = workbook.Worksheets.Add("Auditorias Detalladas");

        var titleRange = wsDetalle.Range("A2:E2");
        titleRange.Merge().Value = "REPORTE DE AUDITORÍA 5S";
        titleRange.Style.Font.Bold = true;
        titleRange.Style.Font.FontSize = 16;
        titleRange.Style.Font.FontName = "Segoe UI";
        titleRange.Style.Font.FontColor = XLColor.White;
        titleRange.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#1E293B"));
        titleRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        titleRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        wsDetalle.Row(2).Height = 35;

        wsDetalle.Cell("A4").Value = "Módulo:";
        wsDetalle.Cell("B4").Value = data.ModuleName;
        wsDetalle.Cell("A5").Value = "Área / Línea:";
        wsDetalle.Cell("B5").Value = data.AreaName;
        wsDetalle.Cell("A7").Value = "Auditor:";
        wsDetalle.Cell("B7").Value = data.AuditorName;
        wsDetalle.Cell("A8").Value = "Fecha Registro:";
        wsDetalle.Cell("B8").Value = data.AuditDate.ToString("dd/MM/yyyy HH:mm:ss");

        var labelRange = wsDetalle.Range("A4:A8");
        labelRange.Style.Font.Bold = true;
        labelRange.Style.Font.FontColor = XLColor.FromHtml("#475569");

        var scoreLabel = wsDetalle.Cell("E4");
        scoreLabel.Value = "PUNTAJE FINAL";
        scoreLabel.Style.Font.Bold = true;
        scoreLabel.Style.Font.FontSize = 9;
        scoreLabel.Style.Font.FontColor = XLColor.FromHtml("#64748B");
        scoreLabel.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        var scoreCell = wsDetalle.Cell("E5");
        scoreCell.Value = promedioGeneral100;
        scoreCell.Style.NumberFormat.Format = "0.00";
        scoreCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        scoreCell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        scoreCell.Style.Font.Bold = true;
        scoreCell.Style.Font.FontSize = 24;
        scoreCell.Style.Font.FontColor = cardTextColor;

        wsDetalle.Range("E5:E7").Merge();
        var cardRange = wsDetalle.Range("E4:E7");
        cardRange.Style.Fill.SetBackgroundColor(cardBackground);
        cardRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.FromHtml("#CBD5E1"));

        int startRow = 10;
        wsDetalle.Cell(startRow, 1).Value = "Categoría 5S";
        wsDetalle.Cell(startRow, 2).Value = "ID";
        wsDetalle.Cell(startRow, 3).Value = "Aspecto Evaluado";
        wsDetalle.Cell(startRow, 4).Value = "Puntuación (1-5)";
        wsDetalle.Cell(startRow, 5).Value = "Puntos (x20)";

        var headerRange = wsDetalle.Range(startRow, 1, startRow, 5);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#334155"));
        headerRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        wsDetalle.Row(startRow).Height = 25;

        int currentRow = startRow + 1;
        foreach (var ans in data.Answers)
        {
            int puntosMultiplicados = ans.Score * 20;

            wsDetalle.Cell(currentRow, 1).Value = ans.CategoryTitle;
            wsDetalle.Cell(currentRow, 2).Value = ans.QuestionId;
            wsDetalle.Cell(currentRow, 3).Value = ans.QuestionText;
            wsDetalle.Cell(currentRow, 4).Value = ans.Score;
            wsDetalle.Cell(currentRow, 5).Value = puntosMultiplicados;

            wsDetalle.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            wsDetalle.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.FontName = "Consolas";
            wsDetalle.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            wsDetalle.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            wsDetalle.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.Bold = true;

            if (currentRow % 2 == 0)
            {
                wsDetalle.Range(currentRow, 1, currentRow, 5).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#F8FAFC"));
            }
            wsDetalle.Range(currentRow, 1, currentRow, 5).Style.Border.SetBottomBorder(XLBorderStyleValues.Thin).Border.SetBottomBorderColor(XLColor.FromHtml("#F1F5F9"));
            currentRow++;
        }

        wsDetalle.Columns(1, 5).AdjustToContents();
        wsDetalle.Column(3).Width = 65;
        wsDetalle.Column(3).Style.Alignment.SetWrapText(true);


        var wsResumen = workbook.Worksheets.Add("Resumen");

        var summaryTitleRange = wsResumen.Range("A2:B2");
        summaryTitleRange.Merge().Value = "PROMEDIOS CONSOLIDADOS POR CLASIFICACIÓN";
        summaryTitleRange.Style.Font.Bold = true;
        summaryTitleRange.Style.Font.FontSize = 12;
        summaryTitleRange.Style.Font.FontName = "Segoe UI";
        summaryTitleRange.Style.Font.FontColor = XLColor.White;
        summaryTitleRange.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#475569"));
        summaryTitleRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        summaryTitleRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        wsResumen.Row(2).Height = 28;

        int summaryStartRow = 4;
        wsResumen.Cell(summaryStartRow, 1).Value = "Clasificación 5S";
        wsResumen.Cell(summaryStartRow, 2).Value = "Puntaje Final";

        var summaryHeaderRange = wsResumen.Range(summaryStartRow, 1, summaryStartRow, 2);
        summaryHeaderRange.Style.Font.Bold = true;
        summaryHeaderRange.Style.Font.FontColor = XLColor.White;
        summaryHeaderRange.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#1E293B"));
        summaryHeaderRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        wsResumen.Row(summaryStartRow).Height = 22;

        int rRow = summaryStartRow + 1;
        foreach (var item in resumenPorS)
        {
            wsResumen.Cell(rRow, 1).Value = item.Categoria;
            wsResumen.Cell(rRow, 2).Value = item.Promedio100;

            wsResumen.Cell(rRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Font.Bold = true;
            wsResumen.Cell(rRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.Bold = true;
            wsResumen.Cell(rRow, 2).Style.NumberFormat.Format = "0.00";

            var sTextColor = item.Promedio100 >= 95M ? XLColor.FromHtml("#166534")
                           : item.Promedio100 >= 85M ? XLColor.FromHtml("#075985")
                           : item.Promedio100 >= 70M ? XLColor.FromHtml("#92400E")
                           : XLColor.FromHtml("#991B1B");

            wsResumen.Cell(rRow, 2).Style.Font.FontColor = sTextColor;

            wsResumen.Range(rRow, 1, rRow, 2).Style.Border.SetBottomBorder(XLBorderStyleValues.Thin).Border.SetBottomBorderColor(XLColor.FromHtml("#E2E8F0"));
            rRow++;
        }

        wsResumen.Columns(1, 2).AdjustToContents();
        wsResumen.Column(1).Width = 35;
        wsResumen.Column(2).Width = 26;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}