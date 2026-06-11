using MediatR;

namespace Application.Auditors.Queries.GetActiveAuditors
{
    public record GetActiveAuditorsQuery() : IRequest<IEnumerable<AuditorDto>>;

    public record AuditorDto(int AuditorId, string FullName, bool isActive);
}