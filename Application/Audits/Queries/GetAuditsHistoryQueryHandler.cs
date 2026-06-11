using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Audits.Queries
{
    public class GetAuditsHistoryQueryHandler : IRequestHandler<GetAuditsHistoryQuery, IEnumerable<AuditHistoryDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetAuditsHistoryQueryHandler(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<AuditHistoryDto>> Handle(GetAuditsHistoryQuery request, CancellationToken cancellationToken)
        {
            return await _context.Audits
                .Include(a => a.Auditor)
                .Include(a => a.Area)
                    .ThenInclude(area => area.Module)
                .Include(a => a.Answers)
                .OrderByDescending(a => a.AuditDate)
                .Select(a => new AuditHistoryDto(
                    a.Id,
                    a.Auditor!.FullName,
                    a.Area!.Name,
                    a.Area!.Module!.Name,
                    a.AuditDate,
                    a.FinalScore
                ))
                .ToListAsync(cancellationToken);
        }
    }
}