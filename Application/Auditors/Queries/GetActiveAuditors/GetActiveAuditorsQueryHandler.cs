using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using MediatR;

namespace Application.Auditors.Queries.GetActiveAuditors
{
    public class GetActiveAuditorsQueryHandler : IRequestHandler<GetActiveAuditorsQuery, IEnumerable<AuditorDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetActiveAuditorsQueryHandler(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<AuditorDto>> Handle(GetActiveAuditorsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Auditors
                .Where(a => a.IsActive)
                .OrderBy(a => a.FullName)
                .Select(a => new AuditorDto(a.Id, a.FullName, a.IsActive))
                .ToListAsync(cancellationToken);
        }
    }
}