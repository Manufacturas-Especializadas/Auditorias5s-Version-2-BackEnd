namespace Domain.Entities
{
    public class Category5s
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? JapaneseTerm { get; set; }

        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}