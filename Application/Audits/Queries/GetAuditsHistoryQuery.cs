using MediatR;

namespace Application.Audits.Queries
{
    public record GetAuditsHistoryQuery() : IRequest<IEnumerable<AuditHistoryDto>>;

    public record AuditHistoryDto(
        int AuditId,
        string AuditorName,
        string AreaName,
        string ModuleName,
        DateTime AuditDate,
        decimal? FinalScore
    );
}