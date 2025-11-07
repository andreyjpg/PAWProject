using Microsoft.AspNetCore.Mvc;
using PAWProject.DTOs;
using PAWProject.MVC.Models;
using System.Diagnostics;


namespace PAWProject.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClientRssConsumer;
        private readonly HomeViewModel _homeViewModel;


        public HomeController(ILogger<HomeController> logger, IHttpClientFactory factory)
        {
            _logger = logger;
            _httpClientRssConsumer = factory.CreateClient("RssConsumer");
            _homeViewModel = new HomeViewModel();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var items = await _httpClientRssConsumer.GetFromJsonAsync<List<FeedItemDTO>>("bbcConsumer");
            _homeViewModel.FeedItems = items;
            return View(_homeViewModel);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
