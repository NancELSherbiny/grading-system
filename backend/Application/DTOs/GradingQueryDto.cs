using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

/// <summary>Query-string options for the Grading list of one subject.</summary>
public class GradingQueryDto : QueryDto
{
    [AllowedValues("id", "name", "academicYear", "term1", "term2", "total", ErrorMessage = "Sort by id, name, academicYear, term1, term2 or total.")]
    public string SortBy { get; set; } = "id";
}
