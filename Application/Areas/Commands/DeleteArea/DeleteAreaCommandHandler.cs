using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using MediatR;

namespace Application.Areas.Commands.DeleteArea
{
    public class DeleteAreaCommandHandler : IRequestHandler<DeleteAreaCommand, DeleteAreaResponse>
    {
        private readonly ApplicationDbContext _context;

        public DeleteAreaCommandHandler(ApplicationDbContext context) => _context = context;

        public async Task<DeleteAreaResponse> Handle(DeleteAreaCommand request, CancellationToken cancellationToken)
        {
            var area = await _context.Areas.FirstOrDefaultAsync(a => a.Id == request.AreaId, cancellationToken);
            if (area == null)
                return new DeleteAreaResponse(false, "El área que intenta desactivar no existe.");

            area.IsActive = false;

            await _context.SaveChangesAsync(cancellationToken);
            return new DeleteAreaResponse(true);
        }
    }
}
