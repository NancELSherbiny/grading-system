using DAL.Models;

namespace DAL.Repositories;

public interface IStudentRepository
{
    /// <summary>One page of students (with their marks) matching the filter, plus how many match in total.</summary>
    Task<(List<Student> Items, int TotalCount)> GetPagedAsync(StudentFilter filter);

    /// <summary>A student with their academic year and all their marks.</summary>
    Task<Student?> GetByIdAsync(int id);

    /// <summary>Whether another student already uses this National ID. Pass the student's own id when editing.</summary>
    Task<bool> NationalIdExistsAsync(string nationalId, int? exceptStudentId = null);

    Task AddAsync(Student student);

    void Remove(Student student);

    Task<int> SaveChangesAsync();
}
