namespace Application.Questions.Commands.GetQuestionsByModule
{
    public record ModuleQuestionsResponse(
        string Key,
        string Title,
        IEnumerable<QuestionDto> Questions
    );
}