using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PAWProject.Data.MSSQL;
using PAWProject.Models.Entities;
using PAWProject.MVC.Models;

namespace PAWProject.MVC.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly NewsHubContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            NewsHubContext dbContext,
            IPasswordHasher<User> passwordHasher,
            ILogger<AccountController> logger)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _dbContext.Users
                .Include(u => u.Role)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == model.Username);

            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña inválidos.");
                return View(model);
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña inválidos.");
                return View(model);
            }

            var roleName = user.Role?.RoleName
                ?? user.UserRoles.Select(ur => ur.Role?.RoleName).FirstOrDefault(r => !string.IsNullOrWhiteSpace(r))
                ?? "User";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, roleName)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
            };

            // Limpiamos mensajes previos (ej. registro) para evitar que aparezcan tras iniciar sesión.
            TempData.Remove("Message");

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            await LoadRolesAsync();
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadRolesAsync();
                return View(model);
            }

            var existingUser = await _dbContext.Users.AnyAsync(u => u.Username == model.Username);
            if (existingUser)
            {
                ModelState.AddModelError(nameof(model.Username), "El nombre de usuario ya existe.");
                await LoadRolesAsync();
                return View(model);
            }

            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleId == model.RoleId);
            if (role == null)
            {
                ModelState.AddModelError(nameof(model.RoleId), "Rol inválido.");
                await LoadRolesAsync();
                return View(model);
            }

            var newUser = new User
            {
                Username = model.Username,
                Email = model.Email,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                RoleId = model.RoleId
            };

            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, model.Password);

            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();

            _dbContext.UserRoles.Add(new UserRole
            {
                UserId = newUser.UserId,
                RoleId = model.RoleId
            });
            await _dbContext.SaveChangesAsync();

            TempData["Message"] = "Usuario registrado correctamente. Ahora puede iniciar sesión.";
            return RedirectToAction(nameof(Login));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private async Task LoadRolesAsync()
        {
            var roles = await _dbContext.Roles.AsNoTracking().ToListAsync();
            ViewBag.Roles = new SelectList(roles, "RoleId", "RoleName");
        }
    }
}
