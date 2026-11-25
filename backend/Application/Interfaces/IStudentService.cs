using Application.DTOs;

namespace Application.Interfaces;

public interface IStudentService
{
    Task<PagedResult<StudentDto>> GetPagedAsync(StudentQueryDto query);

    Task<StudentDto> GetByIdAsync(int id);

    Task<StudentGradesDto> GetGradesAsync(int id);

    Task<StudentDto> CreateAsync(SaveStudentDto dto);

    Task<StudentDto> UpdateAsync(int id, SaveStudentDto dto);

    Task DeleteAsync(int id);
}
