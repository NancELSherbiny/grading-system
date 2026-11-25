using Application.DTOs;

namespace Application.Interfaces;

public interface IGradingService
{
    /// <summary>One page of students with their marks in a subject, plus the succeeded and failed counts.</summary>
    Task<GradingPageDto> GetSubjectAsync(int subjectId, GradingQueryDto query);

    /// <summary>Adds or replaces a student's marks in a subject.</summary>
    Task<SubjectMarkDto> SaveMarksAsync(int subjectId, int studentId, SaveMarksDto dto);
}
