using System.ComponentModel.DataAnnotations;

namespace EduProcessManager.ViewModels.Events;

public class EventFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Вкажіть назву")]
    [MaxLength(140)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Вкажіть опис")]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Вкажіть місце")]
    [MaxLength(160)]
    public string Location { get; set; } = string.Empty;

    [Required(ErrorMessage = "Вкажіть дату початку")]
    public DateTime StartAt { get; set; }

    [Required(ErrorMessage = "Вкажіть дату завершення")]
    public DateTime EndAt { get; set; }

    public DateTime? RegistrationAvailableFrom { get; set; }

    public DateTime? RegistrationAvailableUntil { get; set; }

    [Range(1, 500)]
    public int Capacity { get; set; } = 30;

    public bool IsRegistrationOpen { get; set; } = true;
}