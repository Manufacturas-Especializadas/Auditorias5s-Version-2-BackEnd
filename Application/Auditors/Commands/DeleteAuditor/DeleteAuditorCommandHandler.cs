using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Auditors.Commands.DeleteAuditor
{
    public class DeleteAuditorCommandHandler : IRequestHandler<DeleteAuditorCommand, DeleteAuditorResponse>
    {
        private readonly ApplicationDbContext _context;

        public DeleteAuditorCommandHandler(ApplicationDbContext context) => _context = context;

        public async Task<DeleteAuditorResponse> Handle(DeleteAuditorCommand request, CancellationToken cancellationToken)
        {
            var auditor = await _context.Auditors
                .FirstOrDefaultAsync(a => a.Id == request.AuditorId, cancellationToken);

            if (auditor == null)
                return new DeleteAuditorResponse(false, "El auditor que intenta dar de baja no existe.");

            auditor.IsActive = false;

            await _context.SaveChangesAsync(cancellationToken);

            return new DeleteAuditorResponse(true);
        }
    }
}