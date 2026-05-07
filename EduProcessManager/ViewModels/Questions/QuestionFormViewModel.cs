namespace EduProcessManager.ViewModels.Questions;

public class QuestionFormViewModel
{
    public int Id { get; set; }

    public int TestId { get; set; }

    public string TestTitle { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public bool AllowMultipleAnswers { get; set; }

    public int Points { get; set; } = 1;

    public int? CorrectOptionIndex { get; set; }

    public List<AnswerOptionFormViewModel> Options { get; set; } = new()
    {
        new AnswerOptionFormViewModel(),
        new AnswerOptionFormViewModel(),
        new AnswerOptionFormViewModel(),
        new AnswerOptionFormViewModel()
    };
}