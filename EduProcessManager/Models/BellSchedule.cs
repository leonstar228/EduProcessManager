using System.ComponentModel.DataAnnotations;

namespace EduProcessManager.Models;

public class BellSchedule
{
    public int Id { get; set; }

    public int LessonNumber { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    [MaxLength(100)]
    public string? Name { get; set; }
}