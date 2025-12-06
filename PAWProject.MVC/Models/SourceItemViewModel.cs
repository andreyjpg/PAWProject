using PAWProject.DTOs;
using PAWProject.DTOs.DTOs;
using System.Collections.Generic;

namespace PAWProject.MVC.Models
{
   
    public class SourceItemViewModel
    {
        public List<(SourceDTO Source, FeedItemDTO Item, DateTime CreatedAt)> SourceItems { get; set; } = new List<(SourceDTO, FeedItemDTO, DateTime)>();
    }
}