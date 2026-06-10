using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using MediatR;

namespace Application.Questions.Commands.DeleteQuestion
{
    public class DeleteQuestionCommandHandler : IRequestHandler<DeleteQuestionCommand, DeleteQuestionResponse>
    {
        private readonly ApplicationDbContext _context;

        public DeleteQuestionCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DeleteQuestionResponse> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
        {
            var question = await _context.Questions
                .FirstOrDefaultAsync(q => q.Id == request.QuestionId, cancellationToken);

            if (question == null)
                return new DeleteQuestionResponse(false, "La pregunta no existe.");

            question.IsActive = false;

            await _context.SaveChangesAsync(cancellationToken);
            return new DeleteQuestionResponse(true);
        }
    }
}