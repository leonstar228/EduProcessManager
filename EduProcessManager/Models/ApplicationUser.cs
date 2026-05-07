using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EduProcessManager.Models;

public class ApplicationUser : IdentityUser
{
    [Required, MaxLength(80)]
    public string FullName { get; set; } = string.Empty;
    public int? StudentGroupId { get; set; }
    public StudentGroup? StudentGroup { get; set; }
    public bool IsActive { get; set; } = true;
}
