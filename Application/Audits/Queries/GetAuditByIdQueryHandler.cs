using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using MediatR;

namespace Application.Audits.Queries
{
    public class GetAuditByIdQueryHandler : IRequestHandler<GetAuditByIdQuery, AuditDetailsDto?>
    {
        private readonly ApplicationDbContext _context;

        public GetAuditByIdQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AuditDetailsDto?> Handle(GetAuditByIdQuery request, CancellationToken cancellationToken)
        {
            var audit = await _context.Audits
                .Include(a => a.Area)
                .Include(a => a.Answers)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            if (audit == null) return null;

            return new AuditDetailsDto(
                audit.Id,
                audit.AuditorId,
                audit.AreaId,
                audit.Area!.ModuleId,
                audit.Answers.Select(ans => new AuditAnswerDetailDto(
                    ans.QuestionId,
                    ans.Score
                )).ToList()
            );
        }
    }
}