namespace DAL.Models;

/// <summary>School stage such as "1st Primary". Fixed list, seeded by the first migration.</summary>
public class AcademicYear
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
