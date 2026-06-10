using MediatR;

namespace Application.Modules.Queries.GetActiveModules
{
    public record GetActiveModulesQuery() : IRequest<IEnumerable<ModuleDto>>;

    public record ModuleDto(int ModuleId, string Name, bool isActive);
}