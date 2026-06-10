namespace Application.Questions.Commands.CreateQuestion
{
    public record QuestionResponse(
        int QuestionId,
        string QuestionText,
        bool Success,
        string ErrorMessage = ""
    );
}