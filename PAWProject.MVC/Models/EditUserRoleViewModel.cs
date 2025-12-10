using System.ComponentModel.DataAnnotations;

namespace PAWProject.MVC.Models;

public class EditUserRoleViewModel
{
    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string? Email { get; set; }

    [Required(ErrorMessage = "Seleccione un rol.")]
    public int? RoleId { get; set; }
}
