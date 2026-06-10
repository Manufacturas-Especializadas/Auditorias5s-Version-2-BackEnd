using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using MediatR;

namespace Application.Modules.Queries.GetActiveModules
{
    public class GetActiveModulesQueryHandler : IRequestHandler<GetActiveModulesQuery, IEnumerable<ModuleDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetActiveModulesQueryHandler(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<ModuleDto>> Handle(GetActiveModulesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Modules
                .Where(m => m.IsActive)
                .OrderBy(m => m.Id)
                .Select(m => new ModuleDto(m.Id, m.Name))
                .ToListAsync(cancellationToken);
        }
    }
}