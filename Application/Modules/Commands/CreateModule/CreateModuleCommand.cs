using MediatR;

namespace Application.Modules.Commands.CreateModule
{
    public record CreateModuleCommand(string Name) : IRequest<ModuleResponse>;

    public record ModuleResponse(int ModuleId, string Name, bool Success, string ErrorMessage = "");
}
