using PAWProject.DTOs;

namespace PAWProject.MVC.Models
{
    public class HomeViewModel
    {
        public IEnumerable<FeedItemDTO>? FeedItems { get; set; }

    }
}
