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

    public async Task<(List<Student> Items, int TotalCount)> GetPagedAsync(
        string? search, int? academicYearId, string sortBy, bool descending, int pageNumber, int pageSize)
    {
        // AsNoTracking: we only read these rows, so EF doesn't need to watch them for changes.
        IQueryable<Student> query = _context.Students
            .Include(s => s.AcademicYear)
            .AsNoTracking();

        // 1. Search and filter
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

        // 2. Count everything that matches, before taking one page
        var totalCount = await query.CountAsync();

        // 3. Sort. Academic year ids are seeded in school order, so sorting by id sorts by stage.
        var sorted = sortBy switch
        {
            "name" => descending ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name),
            "nationalId" => descending ? query.OrderByDescending(s => s.NationalId) : query.OrderBy(s => s.NationalId),
            "academicYear" => descending ? query.OrderByDescending(s => s.AcademicYearId) : query.OrderBy(s => s.AcademicYearId),
            _ => descending ? query.OrderByDescending(s => s.Id) : query.OrderBy(s => s.Id),
        };

        // 4. Page. ThenBy(Id) keeps the order stable when two rows have the same name or year.
        var items = await sorted
            .ThenBy(s => s.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Student?> GetByIdAsync(int id)
        => await _context.Students
            .Include(s => s.AcademicYear)
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
