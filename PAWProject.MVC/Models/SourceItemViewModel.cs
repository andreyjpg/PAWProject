using System;
using System.Collections.Generic;
using System.Linq;
using PAWProject.DTOs;
using PAWProject.DTOs.DTOs;

namespace PAWProject.MVC.Models
{
   
    public class SourceItemViewModel
    {
        public List<(SourceDTO Source, FeedItemDTO Item, DateTime CreatedAt)> SourceItems { get; set; } = new List<(SourceDTO, FeedItemDTO, DateTime)>();

        public IEnumerable<SourceDTO> Sources { get; set; } = Enumerable.Empty<SourceDTO>();
    }
}
