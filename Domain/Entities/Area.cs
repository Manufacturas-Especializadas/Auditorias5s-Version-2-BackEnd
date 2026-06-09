namespace Domain.Entities
{
    public class Area
    {
        public int Id { get; set; }

        public int ModuleId { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public Module? Module { get; set; }

        public ICollection<Audit> Audits { get; set; } = new List<Audit>();
    }
}
