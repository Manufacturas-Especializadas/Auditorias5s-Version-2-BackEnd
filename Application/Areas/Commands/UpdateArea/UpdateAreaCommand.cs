using MediatR;

namespace Application.Areas.Commands.UpdateArea
{
    public record UpdateAreaCommand(
        int AreaId,
        int ModuleId,
        string Name,
        bool IsActive
    ) : IRequest<UpdateAreaResponse>;

    public record UpdateAreaResponse(bool Success, string ErrorMessage = "");
}
