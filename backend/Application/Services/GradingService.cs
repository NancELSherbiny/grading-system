using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using DAL.Models;
using DAL.Repositories;

namespace Application.Services;

public class GradingService : IGradingService
{
    private readonly IMarkRepository _marks;
    private readonly IStudentRepository _students;
    private readonly ILookupRepository _lookups;

    public GradingService(IMarkRepository marks, IStudentRepository students, ILookupRepository lookups)
    {
        _marks = marks;
        _students = students;
        _lookups = lookups;
    }

    public async Task<GradingPageDto> GetSubjectAsync(int subjectId, GradingQueryDto query)
    {
        var subject = await FindSubjectAsync(subjectId);

        var (students, totalCount) = await _marks.GetSubjectPageAsync(
            subjectId,
            query.Search?.Trim(),
            query.AcademicYearId,
            query.SortBy,
            descending: query.SortDirection == "desc",
            query.PageNumber,
            query.PageSize);

        var minimumPassingTotal = GradeCalculator.MinimumPassingTotal(GradeCalculator.MaxSubjectTotal);
        var (succeeded, failed) = await _marks.CountResultsAsync(subjectId, minimumPassingTotal);

        return new GradingPageDto
        {
            SubjectId = subject.Id,
            Subject = subject.Name,
            Students = new PagedResult<SubjectMarkDto>
            {
                // The repository loads only this subject's mark, so FirstOrDefault is that mark or null.
                Items = students.Select(s => ToDto(s, s.Marks.FirstOrDefault())).ToList(),
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = totalCount,
            },
            Succeeded = succeeded,
            Failed = failed,
        };
    }

    public async Task<SubjectMarkDto> SaveMarksAsync(int subjectId, int studentId, SaveMarksDto dto)
    {
        await FindSubjectAsync(subjectId);
        var student = await _students.GetByIdAsync(studentId)
            ?? throw new NotFoundException($"Student {studentId} was not found.");

        // Add the mark the first time, update it after that: one row per student per subject.
        var mark = await _marks.GetAsync(studentId, subjectId);
        if (mark is null)
        {
            mark = new Mark { StudentId = studentId, SubjectId = subjectId };
            await _marks.AddAsync(mark);
        }

        mark.Term1 = dto.Term1!.Value;
        mark.Term2 = dto.Term2!.Value;
        await _marks.SaveChangesAsync();

        return ToDto(student, mark);
    }

    private async Task<Subject> FindSubjectAsync(int subjectId)
        => await _lookups.GetSubjectAsync(subjectId)
            ?? throw new NotFoundException($"Subject {subjectId} was not found.");

    private static SubjectMarkDto ToDto(Student student, Mark? mark)
    {
        var dto = new SubjectMarkDto
        {
            StudentId = student.Id,
            StudentName = student.Name,
            NationalId = student.NationalId,
            AcademicYear = student.AcademicYear?.Name ?? string.Empty,
        };

        if (mark is not null)
        {
            var total = mark.Term1 + mark.Term2;
            dto.Term1 = mark.Term1;
            dto.Term2 = mark.Term2;
            dto.Total = total;
            dto.Percentage = GradeCalculator.GetPercentage(total, GradeCalculator.MaxSubjectTotal);
            dto.Grade = GradeCalculator.GetGrade(total, GradeCalculator.MaxSubjectTotal);
            dto.Passed = GradeCalculator.IsPassed(total, GradeCalculator.MaxSubjectTotal);
        }

        return dto;
    }
}
