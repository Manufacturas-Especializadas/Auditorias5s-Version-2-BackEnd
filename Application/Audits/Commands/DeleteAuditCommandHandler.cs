using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Audits.Commands
{
    public record DeleteAuditCommand(int id) : IRequest<bool>;

    public class DeleteAuditCommandHandler : IRequestHandler<DeleteAuditCommand, bool>
    {
        private readonly ApplicationDbContext _context;
        public DeleteAuditCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteAuditCommand request, CancellationToken cancellationToken)
        {
            var audit = await _context.Audits
                        .Include(a => a.Answers)
                        .FirstOrDefaultAsync(a => a.Id == request.id, cancellationToken);

            if (audit == null) return false;

            _context.AuditAnswers.RemoveRange(audit.Answers);
            _context.Audits.Remove(audit);

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}