using MediatR;

namespace Application.Audits.Queries
{
    public record AuditAnswerDetailDto(int QuestionId, int Score);

    public record AuditDetailsDto(
        int AuditId,
        int AuditorId,
        int AreaId,
        int ModuleId,
        List<AuditAnswerDetailDto> Answers
    );

    public record GetAuditByIdQuery(int Id) : IRequest<AuditDetailsDto?>;
}