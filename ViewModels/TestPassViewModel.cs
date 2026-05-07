using EduProcessManager.Models;
namespace EduProcessManager.ViewModels;
public class TestPassViewModel
{
    public int TestId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int TimeLimitMinutes { get; set; }
    public List<Question> Questions { get; set; } = new();
    public Dictionary<int, List<int>> SelectedAnswers { get; set; } = new();
}
