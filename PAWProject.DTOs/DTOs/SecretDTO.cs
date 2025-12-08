namespace PAWProject.DTOs.DTOs
{
    public class SecretDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Value { get; set; } = null!;

        public string? Description { get; set; }

        public int? SourceId { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual SourceDTO? Source { get; set; }
    }
}
