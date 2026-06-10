using MediatR;

namespace Application.Auditors.Commands.CreateAuditor
{
    public record CreateAuditorCommand(string FullName) : IRequest<AuditorResponse>;

    public record AuditorResponse(int AuditorId, string FullName, bool Success, string ErrorMessage = "");
}
