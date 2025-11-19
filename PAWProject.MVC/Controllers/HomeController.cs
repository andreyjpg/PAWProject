using Microsoft.AspNetCore.Mvc;
using PAWProject.DTOs;
using PAWProject.MVC.Models;
using PAWProject.MVC.Services;
using System.Diagnostics;
using System.Text.Json;

namespace PAWProject.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INewsIngestionService _newsIngestionService;
        private readonly ISourceStore _sourceStore;
        private readonly ISourceItemStore _sourceItemStore;

        public HomeController(
            ILogger<HomeController> logger,
            INewsIngestionService newsIngestionService,
            ISourceStore sourceStore,
            ISourceItemStore sourceItemStore)
        {
            _logger = logger;
            _newsIngestionService = newsIngestionService;
            _sourceStore = sourceStore;
            _sourceItemStore = sourceItemStore;
        }

        public async Task<IActionResult> Index(int? sourceId)
        {
            var model = new HomeViewModel();

            var sources = _sourceStore.GetAll();
            model.Sources = sources;

            if (sources.Count == 0)
            {
                model.FeedItems = Enumerable.Empty<FeedItemDTO>();
                return View(model);
            }

            var selectedSource = sourceId.HasValue
                ? _sourceStore.GetById(sourceId.Value)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveItem(int sourceId, FeedItemDTO item)
        {
            var source = _sourceStore.GetById(sourceId);
            if (source == null)
            {
                return NotFound();
            }

            var json = JsonSerializer.Serialize(item);

            _sourceItemStore.Add(new SourceItem
            {
                SourceId = sourceId,
                Json = json,
                CreatedAt = DateTime.UtcNow
            });

            TempData["Message"] = "El item se ha guardado correctamente (almacenado en memoria).";

            return RedirectToAction(nameof(Index), new { sourceId });
        }

        [HttpGet]
        public IActionResult Saved(int? sourceId)
        {
            var sources = _sourceStore.GetAll();
            ViewBag.Sources = sources;

            var items = sourceId.HasValue
                ? _sourceItemStore.GetBySourceId(sourceId.Value)
                : _sourceItemStore.GetAll();

            var viewModel = new List<(Source Source, FeedItemDTO Item, DateTime CreatedAt)>();

            foreach (var stored in items)
            {
                try
                {
                    var dto = JsonSerializer.Deserialize<FeedItemDTO>(stored.Json);
                    var source = _sourceStore.GetById(stored.SourceId);
                    if (dto != null && source != null)
                    {
                        viewModel.Add((source, dto, stored.CreatedAt));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al deserializar un item guardado.");
                }
            }

            return View(viewModel);
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