using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Questions.Commands.CreateQuestion
{
    public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, QuestionResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateQuestionCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<QuestionResponse> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.QuestionText))
            {
                return new QuestionResponse(0, string.Empty, false, "El texto de la pregunta no puede estar vacío.");
            }

            if (request.ModuleId <= 0 || request.CategoryId <= 0)
            {
                return new QuestionResponse(0, string.Empty, false, "El módulo y la categoría son obligatorios.");
            }

            int maxOrder = await _context.Questions
                .Where(q => q.ModuleId == request.ModuleId && q.CategoryId == request.CategoryId)
                .Select(q => (int?)q.DisplayOrder) 
                .MaxAsync(cancellationToken) ?? 0;

            int nextDisplayOrder = maxOrder + 1;

            var newQuestion = new Question
            {
                ModuleId = request.ModuleId,
                CategoryId = request.CategoryId,
                QuestionText = request.QuestionText.Trim(),
                DisplayOrder = nextDisplayOrder,
                IsActive = true
            };

            _context.Questions.Add(newQuestion);
            await _context.SaveChangesAsync(cancellationToken);

            return new QuestionResponse(newQuestion.Id, newQuestion.QuestionText, true);
        }
    }
}
