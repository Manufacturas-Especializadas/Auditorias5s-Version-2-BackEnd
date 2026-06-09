namespace Domain.Entities
{
    public class Auditor
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public ICollection<Audit> Audits { get; set; } = new List<Audit>();
    }
}