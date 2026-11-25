namespace Application.DTOs;

/// <summary>A student's grade report: every subject, then the overall result.</summary>
public class StudentGradesDto
{
    public int StudentId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string NationalId { get; set; } = string.Empty;

    public string AcademicYear { get; set; } = string.Empty;

    public List<SubjectGradeDto> Subjects { get; set; } = [];

    // Overall result. Null until every subject has marks.
    public int? Total { get; set; }

    public int MaxTotal { get; set; }

    public decimal? Percentage { get; set; }

    public string? Grade { get; set; }
}

public class SubjectGradeDto
{
    public int SubjectId { get; set; }

    public string Subject { get; set; } = string.Empty;

    // All null when the subject has no marks yet.
    public int? Term1 { get; set; }

    public int? Term2 { get; set; }

    public int? Total { get; set; }

    public string? Grade { get; set; }
}
