using System.ComponentModel.DataAnnotations;

namespace PAWProject.MVC.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Ingrese su nombre de usuario.")]
    [StringLength(255, ErrorMessage = "El usuario supera la cantidad de caracteres permitidos.")]
    [Display(Name = "Usuario")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingrese su contraseña.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
