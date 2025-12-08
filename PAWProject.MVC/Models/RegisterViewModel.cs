using System.ComponentModel.DataAnnotations;

namespace PAWProject.MVC.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Ingrese un nombre de usuario.")]
    [StringLength(255)]
    public string Username { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
    [StringLength(255)]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Ingrese una contraseña.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme la contraseña.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seleccione un rol.")]
    public int RoleId { get; set; }
}
