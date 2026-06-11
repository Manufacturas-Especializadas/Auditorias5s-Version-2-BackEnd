using MediatR;

namespace Application.Areas.Queries.GetAreasByModule
{
    public record GetAreasByModuleQuery(int ModuleId) : IRequest<IEnumerable<AreaDto>>;

    public record AreaDto(int AreaId, string Name, bool isActive, int moduleId);
}
