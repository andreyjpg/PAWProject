using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAWProject.DTOs;
using PAWProject.DTOs.DTOs;
using PAWProject.Models.Entities;
using PAWProject.MVC.Models;
using PAWProject.MVC.Services;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;

namespace PAWProject.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INewsIngestionService _newsIngestionService;
        private readonly HttpClient _httpClientAPI;

        public HomeController(
            ILogger<HomeController> logger,
            INewsIngestionService newsIngestionService,
            IHttpClientFactory httpFactory)
        {
            _logger = logger;
            _newsIngestionService = newsIngestionService;
            _httpClientAPI = httpFactory.CreateClient("API");
        }

        public async Task<IActionResult> Index(int? sourceId)
        {
            var model = new HomeViewModel();

            var sources = await _httpClientAPI.GetFromJsonAsync<IEnumerable<SourceDTO>>("api/Source");

            model.Sources = sources;

            if (sources.Count() == 0)
            {
                model.FeedItems = Enumerable.Empty<FeedItemDTO>();
                return View(model);
            }


            var selectedSource = sourceId.HasValue
                ? sources.FirstOrDefault(s => s.Id == sourceId.Value)
                : sources.FirstOrDefault();

            if (selectedSource == null)
            {
                selectedSource = sources.First();
            }

            model.SelectedSourceId = selectedSource.Id;

            var items = await _newsIngestionService.GetItemsFromSourceAsync(selectedSource);
            model.FeedItems = items;

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveItem(int sourceId, FeedItemDTO item)
        {

            var json = JsonSerializer.Serialize(item);

            var sourceItem = new SourceItemDTO
            {
                SourceId = sourceId,
                Json = json,
                CreatedAt = DateTime.UtcNow
            };

            var payload = JsonSerializer.Serialize(sourceItem);


            var response = await _httpClientAPI.PostAsJsonAsync("api/SourceItem", payload);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "El item se ha guardado correctamente.";
            } else
            {
                TempData["Message"] = "Ha ocurrido un error al intentar guardar el item.";

            }

            return RedirectToAction(nameof(Index), new { sourceId });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Saved(int? sourceId)
        {
            var SourceItemViewModel = new SourceItemViewModel();
            var sources = await _httpClientAPI.GetFromJsonAsync<IEnumerable<SourceDTO>>("api/Source");

            ViewBag.Sources = sources;

            var selectedSource = sourceId.HasValue
                ? sources.FirstOrDefault(s => s.Id == sourceId.Value)
                : sources.FirstOrDefault();

            var response = await _httpClientAPI.GetFromJsonAsync<IEnumerable<SourceItemDTO>>("api/SourceItem");

            if (response != null)
            {
                foreach (var stored in response)
                {
                    try
                    {
                        var dto = JsonSerializer.Deserialize<FeedItemDTO>(stored.Json);
                        var source = sources.FirstOrDefault();
                        if (dto != null && source != null)
                        {
                            SourceItemViewModel.SourceItems.Add((source, dto, stored.CreatedAt));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al deserializar un item guardado.");
                    }
                }
            }

            return View(SourceItemViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Download()
        {
            var response = await _httpClientAPI.GetFromJsonAsync<IEnumerable<SourceItemDTO>>("api/SourceItem");

            if (response == null || !response.Any())
                return BadRequest("No hay datos para exportar.");

            var jsonObjects = response.Select(x => JsonSerializer.Deserialize<JsonElement>(x.Json)).ToList();

            var outputJson = JsonSerializer.Serialize(jsonObjects, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            var bytes = System.Text.Encoding.UTF8.GetBytes(outputJson);

            var fileName = $"noticias_export_{DateTime.UtcNow:yyyyMMdd_HHmm}.json";

            return File(bytes, "application/json", fileName);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
