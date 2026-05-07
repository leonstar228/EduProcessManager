namespace EduProcessManager.Models;

public class StudentGroup
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<ApplicationUser> Students { get; set; } = new List<ApplicationUser>();

    public ICollection<TestStudentGroup> TestStudentGroups { get; set; } = new List<TestStudentGroup>();
}