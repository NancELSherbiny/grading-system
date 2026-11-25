using System.ComponentModel.DataAnnotations;
using DAL.Models;

namespace Application.DTOs;

/// <summary>Term 1 and Term 2 for one student in one subject. Both are required, each out of 30.</summary>
public class SaveMarksDto
{
    // int? + [Required] so a missing value is an error, instead of silently becoming 0.
    [Required(ErrorMessage = "Term 1 is required.")]
    [Range(0, Mark.MaxTermMark, ErrorMessage = "Term 1 must be between {1} and {2}.")]
    public int? Term1 { get; set; }

    [Required(ErrorMessage = "Term 2 is required.")]
    [Range(0, Mark.MaxTermMark, ErrorMessage = "Term 2 must be between {1} and {2}.")]
    public int? Term2 { get; set; }
}
