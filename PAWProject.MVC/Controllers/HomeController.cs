using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAWProject.DTOs;
using PAWProject.DTOs.DTOs;
using PAWProject.Models.Entities;
using PAWProject.MVC.Models;
using PAWProject.MVC.Services;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

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
            var viewModel = new SourceItemViewModel();
            var sources = await _httpClientAPI.GetFromJsonAsync<IEnumerable<SourceDTO>>("api/Source")
                ?? Enumerable.Empty<SourceDTO>();

            viewModel.Sources = sources;

            var response = await _httpClientAPI.GetFromJsonAsync<IEnumerable<SourceItemDTO>>("api/SourceItem");

            if (response != null)
            {
                foreach (var stored in response)
                {
                    try
                    {
                        var dto = JsonSerializer.Deserialize<FeedItemDTO>(stored.Json);
                        var source = sources.FirstOrDefault(s => s.Id == stored.SourceId)
                                     ?? stored.Source
                                     ?? sources.FirstOrDefault();

                        if (dto != null && source != null)
                        {
                            viewModel.SourceItems.Add((source, dto, stored.CreatedAt));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al deserializar un item guardado.");
                    }
                }
            }

            return View(viewModel);
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

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportJson(IFormFile file, int? sourceId)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Message"] = "Seleccione un archivo JSON ";
                return RedirectToAction(nameof(Saved));
            }

            if (sourceId == null)
            {
                TempData["Message"] = "Seleccione una fuente.";
                return RedirectToAction(nameof(Saved));
            }

            var sources = await _httpClientAPI.GetFromJsonAsync<IEnumerable<SourceDTO>>("api/Source")
                ?? Enumerable.Empty<SourceDTO>();
            var selectedSource = sources.FirstOrDefault(s => s.Id == sourceId.Value);

            if (selectedSource == null)
            {
                TempData["Message"] = "La fuente seleccionada no es valida.";
                return RedirectToAction(nameof(Saved));
            }

            List<FeedItemDTO> items = new();
            try
            {
                using var reader = new StreamReader(file.OpenReadStream());
                var content = await reader.ReadToEndAsync();

                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                if (root.ValueKind == JsonValueKind.Array)
                {
                    foreach (var element in root.EnumerateArray())
                    {
                        var item = element.Deserialize<FeedItemDTO>(options);
                        if (item != null)
                        {
                            items.Add(item);
                        }
                    }
                }
                else if (root.ValueKind == JsonValueKind.Object)
                {
                    var single = root.Deserialize<FeedItemDTO>(options);
                    if (single != null)
                    {
                        items.Add(single);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar el archivo JSON importado.");
                TempData["Message"] = "El archivo JSON no tiene un formato válido.";
                return RedirectToAction(nameof(Saved));
            }

            if (!items.Any())
            {
                TempData["Message"] = "No se encontraron items para importar.";
                return RedirectToAction(nameof(Saved));
            }

            int success = 0;
            foreach (var item in items)
            {
                try
                {
                    var json = JsonSerializer.Serialize(item);
                    var sourceItem = new SourceItemDTO
                    {
                        SourceId = selectedSource.Id,
                        Json = json,
                        CreatedAt = DateTime.UtcNow
                    };

                    var payload = JsonSerializer.Serialize(sourceItem);
                    var response = await _httpClientAPI.PostAsJsonAsync("api/SourceItem", payload);
                    if (response.IsSuccessStatusCode)
                    {
                        success++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al importar un item desde JSON.");
                }
            }

            TempData["Message"] = success > 0
                ? $"Se importaron {success} items correctamente."
                : "No se pudieron importar items.";

            return RedirectToAction(nameof(Saved));
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
