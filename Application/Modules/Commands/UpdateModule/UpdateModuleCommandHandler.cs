using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace Application.Modules.Commands.UpdateModule
{
    public class UpdateModuleCommandHandler : IRequestHandler<UpdateModuleCommand, UpdateModuleResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateModuleCommandHandler(ApplicationDbContext context) => _context = context;

        public async Task<UpdateModuleResponse> Handle(UpdateModuleCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return new UpdateModuleResponse(false, "El nombre del módulo no puede estar vacío.");

            var module = await _context.Modules
                .FirstOrDefaultAsync(m => m.Id == request.ModuleId, cancellationToken);

            if (module == null)
                return new UpdateModuleResponse(false, "El módulo especificado no existe.");

            module.Name = request.Name.Trim();
            module.IsActive = request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);
            return new UpdateModuleResponse(true);
        }
    }
}