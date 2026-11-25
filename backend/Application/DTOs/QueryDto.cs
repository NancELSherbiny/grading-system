using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

/// <summary>Search, filter and paging options shared by the Students and Grading lists.</summary>
public abstract class QueryDto
{
    /// <summary>Matches name, National ID or academic year.</summary>
    [StringLength(100)]
    public string? Search { get; set; }

    public int? AcademicYearId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Page number must be 1 or more.")]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100.")]
    public int PageSize { get; set; } = 10;

    [AllowedValues("asc", "desc", ErrorMessage = "Sort direction must be asc or desc.")]
    public string SortDirection { get; set; } = "asc";
}
