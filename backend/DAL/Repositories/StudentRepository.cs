using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _context;

    public StudentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Student> Items, int TotalCount)> GetPagedAsync(StudentFilter filter)
    {
        // AsNoTracking: we only read these rows, so EF doesn't need to watch them for changes.
        var query = _context.Students
            .Include(s => s.AcademicYear)
            .Include(s => s.Marks)
            .AsNoTracking()
            .Search(filter.Search, filter.AcademicYearId);

        // Grade filter: graded in every subject, with a total inside the grade's range.
        if (filter.MinTotal.HasValue)
        {
            query = query.Where(s =>
                s.Marks.Count == filter.SubjectCount &&
                s.Marks.Sum(m => m.Term1 + m.Term2) >= filter.MinTotal);
        }

        if (filter.MaxTotal.HasValue)
        {
            query = query.Where(s => s.Marks.Sum(m => m.Term1 + m.Term2) < filter.MaxTotal);
        }

        var totalCount = await query.CountAsync();

        // Academic year ids are seeded in school order, so sorting by the id sorts by stage.
        var sorted = filter.SortBy switch
        {
            "name" => query.OrderBy(s => s.Name, filter.Descending),
            "nationalId" => query.OrderBy(s => s.NationalId, filter.Descending),
            "academicYear" => query.OrderBy(s => s.AcademicYearId, filter.Descending),
            "total" => query.OrderBy(s => s.Marks.Sum(m => m.Term1 + m.Term2), filter.Descending),
            _ => query.OrderBy(s => s.Id, filter.Descending),
        };

        var items = await sorted.Page(filter.PageNumber, filter.PageSize).ToListAsync();
        return (items, totalCount);
    }

    public async Task<Student?> GetByIdAsync(int id)
        => await _context.Students
            .Include(s => s.AcademicYear)
            .Include(s => s.Marks)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<bool> NationalIdExistsAsync(string nationalId, int? exceptStudentId = null)
        => await _context.Students.AnyAsync(s =>
            s.NationalId == nationalId && (exceptStudentId == null || s.Id != exceptStudentId));

    public async Task AddAsync(Student student)
        => await _context.Students.AddAsync(student);

    public void Remove(Student student)
        => _context.Students.Remove(student);

    public Task<int> SaveChangesAsync()
        => _context.SaveChangesAsync();
}
