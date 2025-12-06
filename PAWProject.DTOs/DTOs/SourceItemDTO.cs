namespace PAWProject.DTOs.DTOs
{
    public class SourceItemDTO
    {
        public int Id { get; set; }

        public int SourceId { get; set; }

        public string Json { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public virtual SourceDTO Source { get; set; } = null!;
    }
}
