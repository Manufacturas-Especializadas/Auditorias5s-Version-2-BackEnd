using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using MediatR;

namespace Application.Areas.Commands.UpdateArea
{
    public class UpdateAreaCommandHandler : IRequestHandler<UpdateAreaCommand, UpdateAreaResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateAreaCommandHandler(ApplicationDbContext context) => _context = context;

        public async Task<UpdateAreaResponse> Handle(UpdateAreaCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return new UpdateAreaResponse(false, "El nombre del área o línea no puede estar vacío.");

            var moduleExists = await _context.Modules.AnyAsync(m => m.Id == request.ModuleId, cancellationToken);
            if (!moduleExists)
                return new UpdateAreaResponse(false, "El módulo seleccionado no es válido.");

            var area = await _context.Areas.FirstOrDefaultAsync(a => a.Id == request.AreaId, cancellationToken);
            if (area == null)
                return new UpdateAreaResponse(false, "El área especificada no existe.");

            area.ModuleId = request.ModuleId;
            area.Name = request.Name.Trim();
            area.IsActive = request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);

            return new UpdateAreaResponse(true);
        }
    }
}