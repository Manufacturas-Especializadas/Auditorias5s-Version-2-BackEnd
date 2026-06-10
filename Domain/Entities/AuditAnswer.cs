namespace Domain.Entities
{
    public class AuditAnswer
    {
        public long Id { get; set; }

        public int AuditId { get; set; }

        public int QuestionId { get; set; }

        public int Score { get; set; }

        public Audit? Audit { get; set; }

        public Question? Question { get; set; }
    }
}