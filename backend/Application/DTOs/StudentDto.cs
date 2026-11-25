namespace Application.DTOs;

/// <summary>A student as returned by the API.</summary>
public class StudentDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string NationalId { get; set; } = string.Empty;

    public int AcademicYearId { get; set; }

    public string AcademicYear { get; set; } = string.Empty;
}
