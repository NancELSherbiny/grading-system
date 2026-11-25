namespace DAL.Models;

public class Student
{
    public const int NameMaxLength = 100;
    public const int NationalIdLength = 14;

    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string NationalId { get; set; } = string.Empty;

    public int AcademicYearId { get; set; }

    public AcademicYear? AcademicYear { get; set; }

    /// <summary>One mark row per subject the student has been graded in.</summary>
    public List<Mark> Marks { get; set; } = [];
}
