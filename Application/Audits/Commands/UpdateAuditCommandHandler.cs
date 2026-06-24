using DocumentFormat.OpenXml.InkML;
using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Audits.Commands
{
    public record UpdateAuditCommand(
        int AuditId,
        int AuditorId,
        int AreaId,
        List<UpdateAuditAnswerDto> Answers 
    ) : IRequest<bool>;

    public record UpdateAuditAnswerDto(int QuestionId, int Score);

    public class UpdateAuditCommandHandler : IRequestHandler<UpdateAuditCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public UpdateAuditCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateAuditCommand request, CancellationToken cancellationToken)
        {
            var audit = await _context.Audits
                .Include(a => a.Answers)
                .FirstOrDefaultAsync(a => a.Id == request.AuditId, cancellationToken);

            if (audit == null) return false;

            if (request.AreaId <= 0 || request.AuditorId <= 0) return false;
            if (request.Answers == null || !request.Answers.Any()) return false;

            var area = await _context.Areas.FirstOrDefaultAsync(a => a.Id == request.AreaId, cancellationToken);
            if (area == null) return false;

            var questionIds = request.Answers.Select(a => a.QuestionId).ToList();
            var databaseQuestions = await _context.Questions
                .Where(q => questionIds.Contains(q.Id) && q.ModuleId == area.ModuleId)
                .ToListAsync(cancellationToken);

            if (databaseQuestions.Count != request.Answers.Count) return false;

            var categoryScores = databaseQuestions
                .GroupBy(q => q.CategoryId)
                .Select(group =>
                {
                    int categoryId = group.Key;
                    var currentGroupQuestionIds = group.Select(q => q.Id).ToList();

                    int obtainedScore = request.Answers
                        .Where(a => currentGroupQuestionIds.Contains(a.QuestionId))
                        .Sum(a => a.Score);

                    int maxPossibleScore = currentGroupQuestionIds.Count * 5;

                    decimal categoryPercentage = maxPossibleScore > 0
                        ? ((decimal)obtainedScore / maxPossibleScore) * 100
                        : 0;

                    return categoryPercentage;
                }).ToList();

            decimal finalScore = categoryScores.Any()
                ? Math.Round(categoryScores.Average(), 1)
                : 0;

            string verdict = finalScore switch
            {
                >= 95 => "Excelente - Cumple con todos los estándares",
                >= 85 => "Bueno - Cumple satisfactoriamente",
                >= 70 => "Regular - Requiere acciones de mejora",
                _ => "Crítico - No cumple con los estándares mínimos"
            };

            audit.AuditorId = request.AuditorId;
            audit.AreaId = request.AreaId;
            audit.FinalScore = finalScore;
            audit.Verdict = verdict;

            _context.AuditAnswers.RemoveRange(audit.Answers);
            audit.Answers.Clear();

            foreach (var ans in request.Answers)
            {
                audit.Answers.Add(new AuditAnswer
                {
                    QuestionId = ans.QuestionId,
                    Score = ans.Score
                });
            }

            _context.Audits.Update(audit);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}