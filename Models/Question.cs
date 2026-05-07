using System.ComponentModel.DataAnnotations;
namespace EduProcessManager.Models;
public class Question
{
    public int Id { get; set; }
    [Required, MaxLength(600)] public string Text { get; set; } = string.Empty;
    public bool AllowMultipleAnswers { get; set; }
    public int Points { get; set; } = 1;
    public int TestId { get; set; }
    public Test? Test { get; set; }
    public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
}
