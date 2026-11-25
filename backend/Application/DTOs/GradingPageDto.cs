namespace Application.DTOs;

/// <summary>Everything the Grading page shows for one subject.</summary>
public class GradingPageDto
{
    public int SubjectId { get; set; }

    public string Subject { get; set; } = string.Empty;

    public PagedResult<SubjectMarkDto> Students { get; set; } = new();

    /// <summary>Students who passed the subject (50% or more). Counts every student, not just this page.</summary>
    public int Succeeded { get; set; }

    /// <summary>Students below 50%. Students without marks in the subject are in neither count.</summary>
    public int Failed { get; set; }
}
