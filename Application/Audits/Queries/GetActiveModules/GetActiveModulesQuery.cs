using MediatR;

namespace Application.Audits.Queries.GetActiveModules
{
    public record GetActiveModulesQuery() : IRequest<IEnumerable<ModuleDto>>;

    public record ModuleDto(int ModuleId, string Name);
}