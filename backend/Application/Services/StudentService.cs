using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using DAL.Models;
using DAL.Repositories;

namespace Application.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _students;
    private readonly ILookupRepository _lookups;

    public StudentService(IStudentRepository students, ILookupRepository lookups)
    {
        _students = students;
        _lookups = lookups;
    }

    public async Task<PagedResult<StudentDto>> GetPagedAsync(StudentQueryDto query)
    {
        var subjectCount = await _lookups.CountSubjectsAsync();

        var filter = new StudentFilter
        {
            Search = query.Search?.Trim(),
            AcademicYearId = query.AcademicYearId,
            SubjectCount = subjectCount,
            SortBy = query.SortBy,
            Descending = query.SortDirection == "desc",
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
        };

        // The grade filter becomes a range of totals, so the database only has to compare numbers.
        if (query.Grade is not null)
        {
            (filter.MinTotal, filter.MaxTotal) =
                GradeCalculator.GetTotalRange(query.Grade, subjectCount * GradeCalculator.MaxSubjectTotal);
        }

        var (items, totalCount) = await _students.GetPagedAsync(filter);

        return new PagedResult<StudentDto>
        {
            Items = items.Select(s => ToDto(s, subjectCount)).ToList(),
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = totalCount,
        };
    }

    public async Task<StudentDto> GetByIdAsync(int id)
    {
        var student = await FindAsync(id);
        return ToDto(student, await _lookups.CountSubjectsAsync());
    }

    public async Task<StudentGradesDto> GetGradesAsync(int id)
    {
        var student = await FindAsync(id);
        var subjects = await _lookups.GetSubjectsAsync();

        // One row per subject, even those without marks yet.
        var rows = subjects.Select(subject =>
        {
            var mark = student.Marks.FirstOrDefault(m => m.SubjectId == subject.Id);
            var total = mark is null ? (int?)null : mark.Term1 + mark.Term2;

            return new SubjectGradeDto
            {
                SubjectId = subject.Id,
                Subject = subject.Name,
                Term1 = mark?.Term1,
                Term2 = mark?.Term2,
                Total = total,
                Grade = total is null ? null : GradeCalculator.GetGrade(total.Value, GradeCalculator.MaxSubjectTotal),
            };
        }).ToList();

        var overall = ToDto(student, subjects.Count);

        return new StudentGradesDto
        {
            StudentId = student.Id,
            Name = student.Name,
            NationalId = student.NationalId,
            AcademicYear = overall.AcademicYear,
            Subjects = rows,
            Total = overall.Total,
            MaxTotal = overall.MaxTotal,
            Percentage = overall.Percentage,
            Grade = overall.Grade,
        };
    }

    public async Task<StudentDto> CreateAsync(SaveStudentDto dto)
    {
        var nationalId = dto.NationalId.Trim();
        await EnsureAcademicYearExistsAsync(dto.AcademicYearId);
        await EnsureNationalIdIsFreeAsync(nationalId);

        var student = new Student
        {
            Name = dto.Name.Trim(),
            NationalId = nationalId,
            AcademicYearId = dto.AcademicYearId,
        };

        await _students.AddAsync(student);
        await _students.SaveChangesAsync();

        // Read it back so the response includes the academic year's name.
        return await GetByIdAsync(student.Id);
    }

    public async Task<StudentDto> UpdateAsync(int id, SaveStudentDto dto)
    {
        var student = await FindAsync(id);

        var nationalId = dto.NationalId.Trim();
        await EnsureAcademicYearExistsAsync(dto.AcademicYearId);
        await EnsureNationalIdIsFreeAsync(nationalId, exceptStudentId: id);

        student.Name = dto.Name.Trim();
        student.NationalId = nationalId;
        student.AcademicYearId = dto.AcademicYearId;
        await _students.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var student = await FindAsync(id);

        // The database deletes the student's marks with them (cascade delete).
        _students.Remove(student);
        await _students.SaveChangesAsync();
    }

    private async Task<Student> FindAsync(int id)
        => await _students.GetByIdAsync(id)
            ?? throw new NotFoundException($"Student {id} was not found.");

    private async Task EnsureAcademicYearExistsAsync(int academicYearId)
    {
        if (!await _lookups.AcademicYearExistsAsync(academicYearId))
        {
            throw new BadRequestException(nameof(SaveStudentDto.AcademicYearId), "The selected academic year does not exist.");
        }
    }

    private async Task EnsureNationalIdIsFreeAsync(string nationalId, int? exceptStudentId = null)
    {
        if (await _students.NationalIdExistsAsync(nationalId, exceptStudentId))
        {
            throw new ConflictException(nameof(SaveStudentDto.NationalId), "A student with this National ID already exists.");
        }
    }

    /// <summary>The overall grade appears only once the student has marks in every subject.</summary>
    private static StudentDto ToDto(Student student, int subjectCount)
    {
        var maxTotal = subjectCount * GradeCalculator.MaxSubjectTotal;
        var fullyGraded = subjectCount > 0 && student.Marks.Count == subjectCount;
        var total = student.Marks.Sum(m => m.Term1 + m.Term2);

        return new StudentDto
        {
            Id = student.Id,
            Name = student.Name,
            NationalId = student.NationalId,
            AcademicYearId = student.AcademicYearId,
            AcademicYear = student.AcademicYear?.Name ?? string.Empty,
            MaxTotal = maxTotal,
            Total = fullyGraded ? total : null,
            Percentage = fullyGraded ? GradeCalculator.GetPercentage(total, maxTotal) : null,
            Grade = fullyGraded ? GradeCalculator.GetGrade(total, maxTotal) : null,
        };
    }
}
