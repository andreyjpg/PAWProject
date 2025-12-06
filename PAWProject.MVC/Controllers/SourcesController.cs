using Microsoft.AspNetCore.Mvc;
using PAWProject.DTOs.DTOs;
using PAWProject.MVC.Models;
using PAWProject.MVC.Services;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace PAWProject.MVC.Controllers
{
   
    public class SourcesController : Controller
    {
        private readonly HttpClient _httpClient;

        public SourcesController( IHttpClientFactory httpFactory)
        {
            _httpClient = httpFactory.CreateClient("API");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = new SourceViewModel
            {
                Sources = await _httpClient.GetFromJsonAsync<IEnumerable<SourceDTO>>("api/Source")
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult Create()
        {

            var vm = new SourceViewModel
            {
                NewSource = new SourceDTO
                {
                    ComponentType = "feed",
                    RequiresSecret = false
                }
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSource(SourceViewModel model)
        {

            if (!string.IsNullOrWhiteSpace(model.NewSource.Url) &&
                !Uri.TryCreate(model.NewSource.Url, UriKind.Absolute, out _))
            {
                ModelState.AddModelError(nameof(model.NewSource.Url), "Ingrese una URL válida (incluya https://).");
            }


            var json = JsonSerializer.Serialize(model.NewSource);

            await _httpClient.PostAsJsonAsync("api/Source", json);

            TempData["Message"] = "Fuente creada correctamente. ";
            return RedirectToAction(nameof(Index));
        }
    }
}