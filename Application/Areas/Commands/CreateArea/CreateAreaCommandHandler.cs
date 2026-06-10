using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using Domain.Entities;
using MediatR;

namespace Application.Areas.Commands.CreateArea
{
    public class CreateAreaCommandHandler : IRequestHandler<CreateAreaCommand, AreaResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateAreaCommandHandler(ApplicationDbContext context) => _context = context;

        public async Task<AreaResponse> Handle(CreateAreaCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return new AreaResponse(0, string.Empty, false, "El nombre del área o línea es requerido.");

            var moduleExists = await _context.Modules.AnyAsync(m => m.Id == request.ModuleId, cancellationToken);
            if (!moduleExists)
                return new AreaResponse(0, string.Empty, false, "El módulo seleccionado no es válido.");

            var newArea = new Area { ModuleId = request.ModuleId, Name = request.Name.Trim(), IsActive = true };

            _context.Areas.Add(newArea);
            await _context.SaveChangesAsync(cancellationToken);

            return new AreaResponse(newArea.Id, newArea.Name, true);
        }
    }
}