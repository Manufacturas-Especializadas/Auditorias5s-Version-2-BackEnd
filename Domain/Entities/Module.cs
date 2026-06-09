namespace Domain.Entities
{
    public class Module
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public ICollection<Area> Areas { get; set; } = new List<Area>();

        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}