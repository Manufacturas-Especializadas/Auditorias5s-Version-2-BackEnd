using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using MediatR;

namespace Application.Auditors.Commands.UpdateAuditor
{
    public class UpdateAuditorCommandHandler : IRequestHandler<UpdateAuditorCommand, UpdateAuditorResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateAuditorCommandHandler(ApplicationDbContext context) => _context = context;

        public async Task<UpdateAuditorResponse> Handle(UpdateAuditorCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.FullName))
                return new UpdateAuditorResponse(false, "El nombre completo del auditor no puede estar vacío.");

            var auditor = await _context.Auditors
                .FirstOrDefaultAsync(a => a.Id == request.AuditorId, cancellationToken);

            if (auditor == null)
                return new UpdateAuditorResponse(false, "El auditor especificado no existe.");

            auditor.FullName = request.FullName.Trim();
            auditor.IsActive = request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);

            return new UpdateAuditorResponse(true);
        }
    }
}