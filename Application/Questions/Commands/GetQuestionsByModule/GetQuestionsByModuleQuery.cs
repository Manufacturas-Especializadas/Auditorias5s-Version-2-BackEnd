using MediatR;

namespace Application.Questions.Commands.GetQuestionsByModule
{
    public record GetQuestionsByModuleQuery(int ModuleId) : IRequest<IEnumerable<ModuleQuestionsResponse>>;
}
