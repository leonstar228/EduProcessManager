using System.ComponentModel.DataAnnotations;

namespace EduProcessManager.Models;

public enum TestScoreScale
{
    Points = 0,
    Percent100 = 1,
    Twelve = 2
}

public class Test
{
    public int Id { get; set; }

    [Required, MaxLength(140)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(800)]
    public string Description { get; set; } = string.Empty;

    public int TimeLimitMinutes { get; set; } = 20;

    public DateTime? AvailableFrom { get; set; }

    public DateTime? AvailableUntil { get; set; }

    public int MaxAttempts { get; set; } = 1;

    public TestScoreScale ScoreScale { get; set; } = TestScoreScale.Points;

    public bool IsPublished { get; set; } = true;

    public string AuthorId { get; set; } = string.Empty;

    public ApplicationUser? Author { get; set; }

    public int SubjectId { get; set; }

    public Subject? Subject { get; set; }

    public ICollection<Question> Questions { get; set; } = new List<Question>();

    public ICollection<TestAttempt> Attempts { get; set; } = new List<TestAttempt>();

    public ICollection<TestStudentGroup> TestStudentGroups { get; set; } = new List<TestStudentGroup>();
}