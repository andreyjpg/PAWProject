using PAWProject.DTOs;
using System.Collections.Generic;

namespace PAWProject.MVC.Models
{
   
    public class HomeViewModel
    {
        public IEnumerable<FeedItemDTO>? FeedItems { get; set; }

        public int? SelectedSourceId { get; set; }

        public IEnumerable<Source>? Sources { get; set; }
    }
}