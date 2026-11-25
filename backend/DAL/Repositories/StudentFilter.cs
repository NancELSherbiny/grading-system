namespace DAL.Repositories;

/// <summary>What the Students list asks the database for: search, filters, sort and page.</summary>
public class StudentFilter
{
    public string? Search { get; set; }

    public int? AcademicYearId { get; set; }

    /// <summary>
    /// Grade filter, already turned into totals by the service: only students with marks in every subject
    /// (<see cref="SubjectCount"/>) and a total from <see cref="MinTotal"/> up to, not including, <see cref="MaxTotal"/>.
    /// </summary>
    public int? MinTotal { get; set; }

    public int? MaxTotal { get; set; }

    public int SubjectCount { get; set; }

    public string SortBy { get; set; } = "id";

    public bool Descending { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
