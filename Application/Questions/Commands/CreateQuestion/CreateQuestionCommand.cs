using MediatR;

namespace Application.Questions.Commands.CreateQuestion
{
    public record CreateQuestionCommand(
        int ModuleId,
        int CategoryId,
        string QuestionText,
        int DisplayOrder
    ) : IRequest<QuestionResponse>;
}