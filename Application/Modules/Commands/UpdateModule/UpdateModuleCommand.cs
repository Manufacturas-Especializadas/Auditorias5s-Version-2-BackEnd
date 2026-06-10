using MediatR;

namespace Application.Modules.Commands.UpdateModule
{
    public record UpdateModuleCommand(int ModuleId, string Name, bool IsActive) : IRequest<UpdateModuleResponse>;

    public record UpdateModuleResponse(bool Success, string ErrorMessage = "");
}