using MediatR;

namespace Application.Audits.Queries
{
    public record GetAuditExcelReportQuery(int AuditId) : IRequest<AuditExcelReportDto?>;

    public record AuditExcelReportDto(
        int AuditId,
        string AuditorName,
        string AreaName,
        string ModuleName,
        DateTime AuditDate,
        decimal? FinalScore,
        List<AuditQuestionReportDto> Answers
    );

    public record AuditQuestionReportDto(
        int QuestionId,
        string CategoryTitle,
        string QuestionText,
        int Score
    );
}