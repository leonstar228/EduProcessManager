namespace EduProcessManager.ViewModels.Questions;

public class AnswerOptionFormViewModel
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }
}