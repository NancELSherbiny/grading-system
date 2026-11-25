using DAL.Models;

namespace DAL.Repositories;

public interface IStudentRepository
{
    /// <summary>One page of students matching the search and filter, plus how many match in total.</summary>
    Task<(List<Student> Items, int TotalCount)> GetPagedAsync(
        string? search, int? academicYearId, string sortBy, bool descending, int pageNumber, int pageSize);

    Task<Student?> GetByIdAsync(int id);

    /// <summary>Whether another student already uses this National ID. Pass the student's own id when editing.</summary>
    Task<bool> NationalIdExistsAsync(string nationalId, int? exceptStudentId = null);

    Task AddAsync(Student student);

    void Remove(Student student);

    Task<int> SaveChangesAsync();
}
