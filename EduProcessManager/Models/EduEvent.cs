using System.ComponentModel.DataAnnotations;
namespace EduProcessManager.Models;

public class EduEvent
{
    public int Id { get; set; }
    [Required, MaxLength(140)] public string Title { get; set; } = string.Empty;
    [Required, MaxLength(1000)] public string Description { get; set; } = string.Empty;
    [Required, MaxLength(160)] public string Location { get; set; } = string.Empty;
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public DateTime? RegistrationAvailableFrom { get; set; }
    public DateTime? RegistrationAvailableUntil { get; set; }
    public int Capacity { get; set; } = 30;
    public bool IsRegistrationOpen { get; set; } = true;
    public string CreatedById { get; set; } = string.Empty;
    public ApplicationUser? CreatedBy { get; set; }
    public ICollection<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();
    public int FreePlaces => Capacity - Registrations.Count(r => r.Status == RegistrationStatus.Confirmed || r.Status == RegistrationStatus.Pending);
}
