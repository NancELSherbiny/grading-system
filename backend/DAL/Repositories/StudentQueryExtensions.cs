using System.Linq.Expressions;
using DAL.Models;

namespace DAL.Repositories;

/// <summary>Search and sort steps shared by the Students list and the Grading list.</summary>
public static class StudentQueryExtensions
{
    /// <summary>Keeps students whose name, National ID or academic year contains the search text, optionally in one academic year.</summary>
    public static IQueryable<Student> Search(this IQueryable<Student> query, string? search, int? academicYearId)
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s =>
                s.Name.Contains(search) ||
                s.NationalId.Contains(search) ||
                s.AcademicYear!.Name.Contains(search));
        }

        if (academicYearId.HasValue)
        {
            query = query.Where(s => s.AcademicYearId == academicYearId.Value);
        }

        return query;
    }

    /// <summary>OrderBy or OrderByDescending, chosen by a flag.</summary>
    public static IOrderedQueryable<Student> OrderBy<TKey>(
        this IQueryable<Student> query, Expression<Func<Student, TKey>> key, bool descending)
        => descending ? query.OrderByDescending(key) : query.OrderBy(key);

    /// <summary>Skips to the requested page and takes one page of rows.</summary>
    public static IQueryable<Student> Page(this IOrderedQueryable<Student> query, int pageNumber, int pageSize)
        // ThenBy(Id) keeps the order stable when two rows tie, so no student shows up on two pages.
        => query.ThenBy(s => s.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize);
}
