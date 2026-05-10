using System.ComponentModel.DataAnnotations;

namespace EduProcessManager.Models;

public class LessonSchedule
{
    public int Id { get; set; }

    public int StudentGroupId { get; set; }

    public StudentGroup StudentGroup { get; set; } = null!;

    [Required, MaxLength(60)]
    public string SubjectName { get; set; } = string.Empty;

    [Required, MaxLength(60)]
    public string TeacherName { get; set; } = string.Empty;

    [Required, MaxLength(30)]
    public string Room { get; set; } = string.Empty;

    public DayOfWeek DayOfWeek { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    [MaxLength(300)]
    public string? Description { get; set; }
}