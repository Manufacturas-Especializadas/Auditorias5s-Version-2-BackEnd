namespace Application.Questions.Commands.GetQuestionsByModule
{
    public record QuestionDto(
        int Id,
        string Text,
        int DisplayOrder,
        bool IsActive
    );
}