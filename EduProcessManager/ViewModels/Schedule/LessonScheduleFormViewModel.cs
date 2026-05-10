using System.ComponentModel.DataAnnotations;
using EduProcessManager.Models;

namespace EduProcessManager.ViewModels.Schedule;

public class LessonScheduleFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Виберіть групу")]
    public int StudentGroupId { get; set; }

    [Required(ErrorMessage = "Вкажіть назву предмета")]
    [StringLength(60, ErrorMessage = "Назва предмета не може бути довша за 50 символів")]
    public string SubjectName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Вкажіть викладача")]
    [StringLength(60, ErrorMessage = "Ім’я викладача не може бути довшим за 50 символів")]
    public string TeacherName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Вкажіть аудиторію")]
    [StringLength(30, ErrorMessage = "Назва аудиторії не може бути довшою за 30 символів")]
    public string Room { get; set; } = string.Empty;

    public DayOfWeek DayOfWeek { get; set; } = DayOfWeek.Monday;

    [Required(ErrorMessage = "Вкажіть час початку")]
    public TimeSpan StartTime { get; set; }

    [Required(ErrorMessage = "Вкажіть час завершення")]
    public TimeSpan EndTime { get; set; }

    [StringLength(300, ErrorMessage = "Опис не може бути довшим за 120 символів")]
    public string? Description { get; set; }

    public List<StudentGroup> Groups { get; set; } = new();
}