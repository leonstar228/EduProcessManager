namespace EduProcessManager.Models;

public class TestStudentGroup
{
    public int TestId { get; set; }

    public Test Test { get; set; } = null!;

    public int StudentGroupId { get; set; }

    public StudentGroup StudentGroup { get; set; } = null!;
}