using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using MediatR;

namespace Application.Questions.Commands.UpdateQuestion
{
    public class UpdateQuestionCommandHandler : IRequestHandler<UpdateQuestionCommand, UpdateQuestionResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateQuestionCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UpdateQuestionResponse> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.QuestionText))
                return new UpdateQuestionResponse(false, "El texto de la pregunta no puede estar vacío.");

            var question = await _context.Questions
                .FirstOrDefaultAsync(q => q.Id == request.QuestionId, cancellationToken);

            if (question == null)
                return new UpdateQuestionResponse(false, "La pregunta especificada no existe.");

            question.ModuleId = request.ModuleId;
            question.CategoryId = request.CategoryId;
            question.QuestionText = request.QuestionText.Trim();
            question.DisplayOrder = request.DisplayOrder;
            question.IsActive = request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);

            return new UpdateQuestionResponse(true);
        }
    }
}