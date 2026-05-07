using EduProcessManager.Models;

namespace EduProcessManager.ViewModels.Tests;

public class TestFormViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int TimeLimitMinutes { get; set; } = 20;

    public int MaxAttempts { get; set; } = 1;

    public TestScoreScale ScoreScale { get; set; } = TestScoreScale.Points;

    public bool IsPublished { get; set; } = true;

    public DateTime? AvailableFrom { get; set; }

    public DateTime? AvailableUntil { get; set; }

    public List<int> SelectedGroupIds { get; set; } = new();

    public List<StudentGroup> Groups { get; set; } = new();
}