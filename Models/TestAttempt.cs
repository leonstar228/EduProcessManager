namespace EduProcessManager.Models;
public class TestAttempt
{
    public int Id { get; set; }
    public int TestId { get; set; }
    public Test? Test { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public ApplicationUser? Student { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.Now;
    public DateTime FinishedAt { get; set; } = DateTime.Now;
    public int Score { get; set; }
    public int MaxScore { get; set; }
    public double Percent => MaxScore == 0 ? 0 : Math.Round((double)Score / MaxScore * 100, 1);
}
