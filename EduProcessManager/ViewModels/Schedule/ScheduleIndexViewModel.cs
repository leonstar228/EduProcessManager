using EduProcessManager.Models;

namespace EduProcessManager.ViewModels.Schedule;

public class ScheduleIndexViewModel
{
    public int? SelectedGroupId { get; set; }

    public string? Search { get; set; }

    public List<StudentGroup> Groups { get; set; } = new();

    public List<LessonSchedule> Lessons { get; set; } = new();

    public List<BellSchedule> Bells { get; set; } = new();

    public DateTime WeekStart { get; set; }
}