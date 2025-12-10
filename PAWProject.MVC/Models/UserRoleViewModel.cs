namespace PAWProject.MVC.Models;

public class UserRoleViewModel
{
    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string RoleName { get; set; } = string.Empty;
}
