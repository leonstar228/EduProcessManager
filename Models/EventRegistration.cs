namespace EduProcessManager.Models;
public class EventRegistration
{
    public int Id { get; set; }
    public int EduEventId { get; set; }
    public EduEvent? EduEvent { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public ApplicationUser? Student { get; set; }
    public DateTime RegisteredAt { get; set; } = DateTime.Now;
    public RegistrationStatus Status { get; set; } = RegistrationStatus.Pending;
}
public enum RegistrationStatus { Pending, Confirmed, Cancelled }
