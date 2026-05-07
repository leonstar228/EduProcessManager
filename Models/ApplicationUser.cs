using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EduProcessManager.Models;

public class ApplicationUser : IdentityUser
{
    [Required, MaxLength(80)]
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
