using Infrastructure.Persistence;
using Domain.Entities;
using MediatR;

namespace Application.Auditors.Commands.CreateAuditor
{
    public class CreateAuditorCommandHandler : IRequestHandler<CreateAuditorCommand, AuditorResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateAuditorCommandHandler(ApplicationDbContext context) => _context = context;

        public async Task<AuditorResponse> Handle(CreateAuditorCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.FullName))
                return new AuditorResponse(0, string.Empty, false, "El nombre completo del auditor es requerido.");

            var auditor = new Auditor
            {
                FullName = request.FullName.Trim(),
                IsActive = true
            };

            _context.Auditors.Add(auditor);
            await _context.SaveChangesAsync(cancellationToken);

            return new AuditorResponse(auditor.Id, auditor.FullName, true);
        }
    }
}