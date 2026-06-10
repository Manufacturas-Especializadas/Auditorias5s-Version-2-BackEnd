using Infrastructure.Persistence;
using MediatR;

namespace Application.Modules.Commands.CreateModule
{
    public class CreateModuleCommandHandler : IRequestHandler<CreateModuleCommand, ModuleResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateModuleCommandHandler(ApplicationDbContext context) => _context = context;

        public async Task<ModuleResponse> Handle(CreateModuleCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return new ModuleResponse(0, string.Empty, false, "El nombre del módulo es requerido.");

            var newModule = new Domain.Entities.Module
            {
                Name = request.Name.Trim(),
                IsActive = true
            };

            _context.Modules.Add(newModule);
            await _context.SaveChangesAsync(cancellationToken);

            return new ModuleResponse(newModule.Id, newModule.Name, true);
        }
    }
}