using MediatR;

namespace Application.Questions.Commands.UpdateQuestion
{
    public record UpdateQuestionCommand(
        int QuestionId,
        int ModuleId,
        int CategoryId,
        string QuestionText,
        int DisplayOrder,
        bool IsActive
    ) : IRequest<UpdateQuestionResponse>;
}
