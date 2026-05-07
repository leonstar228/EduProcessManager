using System.ComponentModel.DataAnnotations;
namespace EduProcessManager.Models;
public class Subject
{
    public int Id { get; set; }
    [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
    [MaxLength(500)] public string Description { get; set; } = string.Empty;
    public ICollection<Test> Tests { get; set; } = new List<Test>();
}
