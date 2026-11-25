using DAL.Models;

namespace DAL.Repositories;

public interface IMarkRepository
{
    /// <summary>
    /// One page of students for the Grading page. Each student's Marks list holds only their mark in this
    /// subject, or is empty if they haven't been graded in it yet.
    /// </summary>
    Task<(List<Student> Items, int TotalCount)> GetSubjectPageAsync(
        int subjectId, string? search, int? academicYearId, string sortBy, bool descending, int pageNumber, int pageSize);

    /// <summary>How many students passed and failed the subject. Students without marks count as neither.</summary>
    Task<(int Succeeded, int Failed)> CountResultsAsync(int subjectId, int minimumPassingTotal);

    Task<Mark?> GetAsync(int studentId, int subjectId);

    Task AddAsync(Mark mark);

    Task<int> SaveChangesAsync();
}
