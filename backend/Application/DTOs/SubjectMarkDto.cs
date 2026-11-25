namespace Application.DTOs;

/// <summary>One row of the Grading page: a student's marks in the selected subject.</summary>
public class SubjectMarkDto
{
    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string NationalId { get; set; } = string.Empty;

    public string AcademicYear { get; set; } = string.Empty;

    // All null when the student has no marks in this subject yet.
    public int? Term1 { get; set; }

    public int? Term2 { get; set; }

    public int? Total { get; set; }

    public decimal? Percentage { get; set; }

    public string? Grade { get; set; }

    public bool? Passed { get; set; }
}
