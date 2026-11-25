using System.ComponentModel.DataAnnotations;
using DAL.Models;

namespace Application.DTOs;

/// <summary>Body for adding or editing a student.</summary>
public class SaveStudentDto
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(Student.NameMaxLength, ErrorMessage = "Name must not exceed {1} characters.")]
    public string Name { get; set; } = string.Empty;

    // [0-9] rather than \d: in .NET, \d also matches Arabic-Indic digits.
    [Required(ErrorMessage = "National ID is required.")]
    [RegularExpression("^[0-9]{14}$", ErrorMessage = "National ID must contain exactly 14 digits.")]
    public string NationalId { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Academic year is required.")]
    public int AcademicYearId { get; set; }
}
