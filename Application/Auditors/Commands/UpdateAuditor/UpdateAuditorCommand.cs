using MediatR;

namespace Application.Auditors.Commands.UpdateAuditor
{
    public record UpdateAuditorCommand(
        int AuditorId,
        string FullName,
        bool IsActive
    ) : IRequest<UpdateAuditorResponse>;

    public record UpdateAuditorResponse(bool Success, string ErrorMessage = "");
}