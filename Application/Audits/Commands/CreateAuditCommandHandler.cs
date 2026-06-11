using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Audits.Commands
{
    public class CreateAuditCommandHandler : IRequestHandler<CreateAuditCommand, CreateAuditResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateAuditCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CreateAuditResponse> Handle(CreateAuditCommand request, CancellationToken cancellationToken)
        {
            if (request.AreaId <= 0 || request.AuditorId <= 0)
                return new CreateAuditResponse(0, 0, string.Empty, false, "El área y el auditor son campos obligatorios.");

            if (request.Answers == null || !request.Answers.Any())
                return new CreateAuditResponse(0, 0, string.Empty, false, "No se pueden guardar auditorías sin respuestas.");

            var area = await _context.Areas.FirstOrDefaultAsync(a => a.Id == request.AreaId, cancellationToken);
            if (area == null)
                return new CreateAuditResponse(0, 0, string.Empty, false, "El área seleccionada no existe.");

            var questionIds = request.Answers.Select(a => a.QuestionId).ToList();
            var databaseQuestions = await _context.Questions
                .Where(q => questionIds.Contains(q.Id) && q.ModuleId == area.ModuleId)
                .ToListAsync(cancellationToken);

            if (databaseQuestions.Count != request.Answers.Count)
                return new CreateAuditResponse(0, 0, string.Empty, false, "Algunas preguntas no pertenecen al módulo de esta área o no existen.");

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

            TimeZoneInfo mexicoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");

            DateTime nowInMexico = TimeZoneInfo.ConvertTime(DateTime.UtcNow, mexicoTimeZone);

            var audit = new Audit
            {
                AreaId = request.AreaId,
                AuditorId = request.AuditorId,
                AuditDate = nowInMexico,
                FinalScore = finalScore,
                Verdict = verdict
            };

            foreach (var ans in request.Answers)
            {
                audit.Answers.Add(new AuditAnswer
                {
                    QuestionId = ans.QuestionId,
                    Score = ans.Score
                });
            }

            _context.Audits.Add(audit);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreateAuditResponse(audit.Id, finalScore, verdict, true);
        }
    }
}