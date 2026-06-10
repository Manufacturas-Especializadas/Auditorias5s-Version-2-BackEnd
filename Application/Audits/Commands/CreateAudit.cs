using MediatR;

namespace Application.Audits.Commands
{
    public record CreateAuditCommand(
        int AreaId,
        int AuditorId,
        List<AuditAnswerInput> Answers
    ) : IRequest<CreateAuditResponse>;

    public record AuditAnswerInput(
        int QuestionId,
        int Score
    );

    public record CreateAuditResponse(
        int AuditId,
        decimal FinalScore,
        string Verdict,
        bool Success,
        string ErrorMessage = ""
    );
}