using MediatR;

namespace Application.Areas.Commands.DeleteArea
{
    public record DeleteAreaCommand(int AreaId) : IRequest<DeleteAreaResponse>;

    public record DeleteAreaResponse(bool Success, string ErrorMessage = "");
}
