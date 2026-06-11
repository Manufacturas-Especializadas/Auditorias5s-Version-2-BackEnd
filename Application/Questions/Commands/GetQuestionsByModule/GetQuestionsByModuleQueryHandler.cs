using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using MediatR;

namespace Application.Questions.Commands.GetQuestionsByModule
{
    public class GetQuestionsByModuleQueryHandler : IRequestHandler<GetQuestionsByModuleQuery, IEnumerable<ModuleQuestionsResponse>>
    {
        private readonly ApplicationDbContext _context;

        public GetQuestionsByModuleQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ModuleQuestionsResponse>> Handle(GetQuestionsByModuleQuery request, CancellationToken cancellationToken)
        {
            var questions = await _context.Questions
                .Include(q => q.Category)
                .Where(q => q.ModuleId == request.ModuleId && q.IsActive)
                .OrderBy(q => q.CategoryId)
                .ThenBy(q => q.DisplayOrder)
                .ToListAsync(cancellationToken);

            var response = questions
                .GroupBy(q => q.CategoryId)
                .Select(group => {
                    var category = group.First().Category;
                    string categoryName = category?.Name ?? "General";
                    string japanese = category?.JapaneseTerm ?? "";

                    return new ModuleQuestionsResponse(
                        Key: categoryName.ToLower().Replace(" ", ""),
                        Title: $"Auditoría - {categoryName} ({japanese})",
                        Questions: group.Select(q => new QuestionDto(
                            q.Id,
                            q.QuestionText,
                            q.DisplayOrder,
                            q.IsActive
                        ))
                    );
                });

            return response;
        }
    }
}