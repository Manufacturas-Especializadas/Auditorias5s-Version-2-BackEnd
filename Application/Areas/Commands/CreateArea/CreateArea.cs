using MediatR;

namespace Application.Areas.Commands.CreateArea
{
    public record CreateAreaCommand(int ModuleId, string Name) : IRequest<AreaResponse>;

    public record AreaResponse(int AreaId, string Name, bool Success, string ErrorMessage = "");
}