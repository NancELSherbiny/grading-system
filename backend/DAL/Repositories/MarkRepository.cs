using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class MarkRepository : IMarkRepository
{
    private readonly AppDbContext _context;

    public MarkRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Student> Items, int TotalCount)> GetSubjectPageAsync(
        int subjectId, string? search, int? academicYearId, string sortBy, bool descending, int pageNumber, int pageSize)
    {
        var query = _context.Students
            .Include(s => s.AcademicYear)
            .Include(s => s.Marks.Where(m => m.SubjectId == subjectId)) // only this subject's mark
            .AsNoTracking()
            .Search(search, academicYearId);

        var totalCount = await query.CountAsync();

        // Students without a mark in this subject get null and sort before everyone else.
        var sorted = sortBy switch
        {
            "name" => query.OrderBy(s => s.Name, descending),
            "academicYear" => query.OrderBy(s => s.AcademicYearId, descending),
            "term1" => query.OrderBy(s => s.Marks.Where(m => m.SubjectId == subjectId).Select(m => (int?)m.Term1).FirstOrDefault(), descending),
            "term2" => query.OrderBy(s => s.Marks.Where(m => m.SubjectId == subjectId).Select(m => (int?)m.Term2).FirstOrDefault(), descending),
            "total" => query.OrderBy(s => s.Marks.Where(m => m.SubjectId == subjectId).Select(m => (int?)(m.Term1 + m.Term2)).FirstOrDefault(), descending),
            _ => query.OrderBy(s => s.Id, descending),
        };

        var items = await sorted.Page(pageNumber, pageSize).ToListAsync();
        return (items, totalCount);
    }

    public async Task<(int Succeeded, int Failed)> CountResultsAsync(int subjectId, int minimumPassingTotal)
    {
        var marks = _context.Marks.Where(m => m.SubjectId == subjectId);

        var succeeded = await marks.CountAsync(m => m.Term1 + m.Term2 >= minimumPassingTotal);
        var failed = await marks.CountAsync(m => m.Term1 + m.Term2 < minimumPassingTotal);
        return (succeeded, failed);
    }

    public async Task<Mark?> GetAsync(int studentId, int subjectId)
        => await _context.Marks.FirstOrDefaultAsync(m => m.StudentId == studentId && m.SubjectId == subjectId);

    public async Task AddAsync(Mark mark)
        => await _context.Marks.AddAsync(mark);

    public Task<int> SaveChangesAsync()
        => _context.SaveChangesAsync();
}
