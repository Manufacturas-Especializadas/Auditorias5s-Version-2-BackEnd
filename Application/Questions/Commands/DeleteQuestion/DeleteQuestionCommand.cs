using MediatR;

namespace Application.Questions.Commands.DeleteQuestion
{
    public record DeleteQuestionCommand(int QuestionId) : IRequest<DeleteQuestionResponse>;
}
