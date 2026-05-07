using System.ComponentModel.DataAnnotations;

namespace EduProcessManager.ViewModels.Account;

public class LoginViewModel
{
    [Required(ErrorMessage = "Вкажіть email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Вкажіть пароль")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}