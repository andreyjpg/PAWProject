using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PAWProject.Data.MSSQL;
using PAWProject.Models.Entities;
using PAWProject.MVC.Models;

namespace PAWProject.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly NewsHubContext _dbContext;
        private readonly ILogger<UsersController> _logger;

        public UsersController(NewsHubContext dbContext, ILogger<UsersController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _dbContext.Users
                .Include(u => u.Role)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .AsNoTracking()
                .ToListAsync();

            var model = users
                .Select(u => new UserRoleViewModel
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    RoleName = GetRoleName(u)
                })
                .ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(int id)
        {
            var user = await _dbContext.Users
                .Include(u => u.Role)
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            var currentRoleId = GetRoleId(user);
            if (currentRoleId == null)
            {
                var defaultRole = await _dbContext.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.RoleName == "User");
                currentRoleId = defaultRole?.RoleId;
            }

        var model = new EditUserRoleViewModel
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            RoleId = currentRoleId
        };

            await LoadRolesAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(EditUserRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadRolesAsync();
                return View(model);
            }

            var user = await _dbContext.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.UserId == model.UserId);

            if (user == null)
        {
            return NotFound();
        }

        if (!model.RoleId.HasValue)
        {
            ModelState.AddModelError(nameof(model.RoleId), "Rol invalido.");
            await LoadRolesAsync();
            return View(model);
        }

        var roleId = model.RoleId.Value;
        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleId == roleId);
        if (role == null)
        {
            ModelState.AddModelError(nameof(model.RoleId), "Rol invalido.");
            await LoadRolesAsync();
            return View(model);
        }

        user.RoleId = roleId;

        if (user.UserRoles.Any())
        {
            foreach (var userRole in user.UserRoles)
            {
                userRole.RoleId = roleId;
            }
        }
        else
        {
            _dbContext.UserRoles.Add(new UserRole
            {
                UserId = user.UserId,
                RoleId = roleId
            });
        }

            await _dbContext.SaveChangesAsync();

            TempData["Message"] = "Rol actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private static string GetRoleName(User user)
        {
            return user.Role?.RoleName
                ?? user.UserRoles.Select(ur => ur.Role?.RoleName).FirstOrDefault(r => !string.IsNullOrWhiteSpace(r))
                ?? "User";
        }

        private static int? GetRoleId(User user)
        {
            if (user.RoleId.HasValue)
            {
                return user.RoleId.Value;
            }

            return user.UserRoles.Select(ur => (int?)ur.RoleId).FirstOrDefault();
        }

        private async Task LoadRolesAsync()
        {
            var roles = await _dbContext.Roles.AsNoTracking().ToListAsync();
            ViewBag.Roles = new SelectList(roles, "RoleId", "RoleName");
        }
    }
}
