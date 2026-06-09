namespace Domain.Entities
{
    public class Audit
    {
        public int Id { get; set; }

        public int AreaId { get; set; }

        public int AuditorId { get; set; }

        public DateTime AuditDate { get; set; } = DateTime.UtcNow;

        public decimal? FinalScore { get; set; }

        public string? Verdict { get; set; }

        public Area? Area { get; set; }

        public Auditor? Auditor { get; set; }

        public ICollection<AuditAnswer> Answers { get; set; } = new List<AuditAnswer>();
    }
}