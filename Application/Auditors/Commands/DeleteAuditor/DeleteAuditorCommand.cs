using MediatR;

namespace Application.Auditors.Commands.DeleteAuditor
{
    public record DeleteAuditorCommand(int AuditorId) : IRequest<DeleteAuditorResponse>;

    public record DeleteAuditorResponse(bool Success, string ErrorMessage = "");
}