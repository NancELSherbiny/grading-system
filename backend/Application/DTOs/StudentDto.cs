namespace Application.DTOs;

/// <summary>A student as returned by the API, with their overall result.</summary>
public class StudentDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string NationalId { get; set; } = string.Empty;

    public int AcademicYearId { get; set; }

    public string AcademicYear { get; set; } = string.Empty;

    // Overall result across all subjects. Null until the student has marks in every subject.
    public int? Total { get; set; }

    public int MaxTotal { get; set; }

    public decimal? Percentage { get; set; }

    public string? Grade { get; set; }
}
