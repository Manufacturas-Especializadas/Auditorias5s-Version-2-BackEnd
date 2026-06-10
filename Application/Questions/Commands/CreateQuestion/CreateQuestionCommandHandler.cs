using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;

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
                return new QuestionResponse(0, string.Empty, false, "El texto de la pregunta no puede estar vacio");
            }

            if (request.ModuleId <= 0 || request.CategoryId <= 0)
            {
                return new QuestionResponse(0, string.Empty, false, "El modulo y la categoría son obligatorios");
            }

            var newQuestions = new Question
            {
                ModuleId = request.ModuleId,
                CategoryId = request.CategoryId,
                QuestionText = request.QuestionText.Trim(),
                DisplayOrder = request.DisplayOrder,
                IsActive = true
            };

            _context.Questions.Add(newQuestions);
            await _context.SaveChangesAsync();

            return new QuestionResponse(newQuestions.Id, newQuestions.QuestionText, true);
        }
    }
}
