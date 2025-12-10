using System.ComponentModel.DataAnnotations;

namespace PAWProject.MVC.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Ingrese un nombre de usuario.")]
    [StringLength(255)]
    public string Username { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Ingrese un correo vA­lido.")]
    [StringLength(255)]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Ingrese una contraseAña.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseAña debe tener al menos 6 caracteres.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme la contraseAña.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Las contraseAñas no coinciden.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
