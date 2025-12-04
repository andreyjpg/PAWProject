using System;

namespace PAWProject.MVC.Models
{
  
    public class SourceItem
    {
        public int Id { get; set; }
        public int SourceId { get; set; }
        public string Json { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}