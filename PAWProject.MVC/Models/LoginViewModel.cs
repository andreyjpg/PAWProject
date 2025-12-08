using System.ComponentModel.DataAnnotations;

namespace PAWProject.MVC.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Ingrese su nombre de usuario.")]
    [StringLength(255)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingrese su contraseña.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
