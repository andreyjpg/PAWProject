using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAWProject.MVC.Models;
using PAWProject.MVC.Services;

namespace PAWProject.MVC.Controllers
{
   
    [Authorize(Roles = "Admin")]
    public class SourcesController : Controller
    {
        private readonly ISourceStore _sourceStore;

        public SourcesController(ISourceStore sourceStore)
        {
            _sourceStore = sourceStore;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var sources = _sourceStore.GetAll();
            return View(sources);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new Source
            {
                ComponentType = "feed",
                RequiresSecret = false
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Source model)
        {
            if (!string.IsNullOrWhiteSpace(model.Url) &&
                !Uri.TryCreate(model.Url, UriKind.Absolute, out _))
            {
                ModelState.AddModelError(nameof(model.Url), "Ingrese una URL válida (incluya https://).");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _sourceStore.Add(model);
            TempData["Message"] = "Fuente creada correctamente (almacenada en memoria). " +
                                  "Cuando se conecte la base de datos, esta misma pantalla guardará en la tabla Sources.";
            return RedirectToAction(nameof(Index));
        }
    }
}
