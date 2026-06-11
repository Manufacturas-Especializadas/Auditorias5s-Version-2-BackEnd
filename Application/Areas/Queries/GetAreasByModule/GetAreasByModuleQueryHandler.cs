using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using MediatR;

namespace Application.Areas.Queries.GetAreasByModule
{
    public class GetAreasByModuleQueryHandler : IRequestHandler<GetAreasByModuleQuery, IEnumerable<AreaDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetAreasByModuleQueryHandler(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<AreaDto>> Handle(GetAreasByModuleQuery request, CancellationToken cancellationToken)
        {
            return await _context.Areas
                .Where(a => a.ModuleId == request.ModuleId && a.IsActive)
                .OrderBy(a => a.Name)
                .Select(a => new AreaDto(a.Id, a.Name, a.IsActive, a.ModuleId))
                .ToListAsync(cancellationToken);
        }
    }
}