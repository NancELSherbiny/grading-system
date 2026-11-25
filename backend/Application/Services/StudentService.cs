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
        var (items, totalCount) = await _students.GetPagedAsync(
            query.Search?.Trim(),
            query.AcademicYearId,
            query.SortBy,
            descending: query.SortDirection == "desc",
            query.PageNumber,
            query.PageSize);

        return new PagedResult<StudentDto>
        {
            Items = items.Select(ToDto).ToList(),
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = totalCount,
        };
    }

    public async Task<StudentDto> GetByIdAsync(int id)
    {
        var student = await _students.GetByIdAsync(id)
            ?? throw new NotFoundException($"Student {id} was not found.");

        return ToDto(student);
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
        var student = await _students.GetByIdAsync(id)
            ?? throw new NotFoundException($"Student {id} was not found.");

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
        var student = await _students.GetByIdAsync(id)
            ?? throw new NotFoundException($"Student {id} was not found.");

        // The database deletes the student's marks with them (cascade delete).
        _students.Remove(student);
        await _students.SaveChangesAsync();
    }

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

    private static StudentDto ToDto(Student student) => new()
    {
        Id = student.Id,
        Name = student.Name,
        NationalId = student.NationalId,
        AcademicYearId = student.AcademicYearId,
        AcademicYear = student.AcademicYear?.Name ?? string.Empty,
    };
}
