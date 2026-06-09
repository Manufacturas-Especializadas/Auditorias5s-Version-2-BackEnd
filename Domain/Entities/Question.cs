namespace Domain.Entities
{
    public class Question
    {
        public int Id { get; set; }

        public int ModuleId { get; set; }

        public int CategoryId { get; set; }

        public string QuestionText { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public Module? Module { get; set; }

        public Category5S? Category { get; set; }

        public ICollection<AuditAnswer> AuditAnswers { get; set; } = new List<AuditAnswer>();
    }
}