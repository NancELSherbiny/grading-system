using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

/// <summary>Query-string options for the Students list.</summary>
public class StudentQueryDto : QueryDto
{
    /// <summary>Overall grade: only students graded in every subject, with this grade.</summary>
    [RegularExpression("^[ABCDF]$", ErrorMessage = "Grade must be A, B, C, D or F.")]
    public string? Grade { get; set; }

    [AllowedValues("id", "name", "nationalId", "academicYear", "total", ErrorMessage = "Sort by id, name, nationalId, academicYear or total.")]
    public string SortBy { get; set; } = "id";
}
