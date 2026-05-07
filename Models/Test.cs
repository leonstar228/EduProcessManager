using System.ComponentModel.DataAnnotations;
namespace EduProcessManager.Models;
public class Test
{
    public int Id { get; set; }
    [Required, MaxLength(140)] public string Title { get; set; } = string.Empty;
    [MaxLength(800)] public string Description { get; set; } = string.Empty;
    public int TimeLimitMinutes { get; set; } = 20;
    public bool IsPublished { get; set; } = true;
    public string AuthorId { get; set; } = string.Empty;
    public ApplicationUser? Author { get; set; }
    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<TestAttempt> Attempts { get; set; } = new List<TestAttempt>();
}
