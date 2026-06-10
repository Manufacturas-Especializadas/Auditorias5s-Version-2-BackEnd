using MediatR;

namespace Application.Modules.Commands.DeleteModule
{
    public record DeleteModuleCommand(int ModuleId) : IRequest<DeleteModuleResponse>;

    public record DeleteModuleResponse(bool Success, string ErrorMessage = "");
}