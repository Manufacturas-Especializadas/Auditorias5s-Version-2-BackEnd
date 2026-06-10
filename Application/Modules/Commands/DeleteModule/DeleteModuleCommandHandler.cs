using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using MediatR;

namespace Application.Modules.Commands.DeleteModule
{
    public class DeleteModuleCommandHandler : IRequestHandler<DeleteModuleCommand, DeleteModuleResponse>
    {
        private readonly ApplicationDbContext _context;

        public DeleteModuleCommandHandler(ApplicationDbContext context) => _context = context;

        public async Task<DeleteModuleResponse> Handle(DeleteModuleCommand request, CancellationToken cancellationToken)
        {
            var module = await _context.Modules
                .FirstOrDefaultAsync(m => m.Id == request.ModuleId, cancellationToken);

            if (module == null)
                return new DeleteModuleResponse(false, "El módulo que intenta desactivar no existe.");

            module.IsActive = false;

            await _context.SaveChangesAsync(cancellationToken);
            return new DeleteModuleResponse(true);
        }
    }
}
