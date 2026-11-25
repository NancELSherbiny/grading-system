namespace Application.DTOs;

/// <summary>Dropdown data: academic years and subjects.</summary>
public class LookupsDto
{
    public List<LookupItemDto> AcademicYears { get; set; } = [];

    public List<LookupItemDto> Subjects { get; set; } = [];
}

public class LookupItemDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
