namespace DAL.Models;

/// <summary>A subject every student is examined in. Fixed list, seeded by the first migration.</summary>
public class Subject
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
