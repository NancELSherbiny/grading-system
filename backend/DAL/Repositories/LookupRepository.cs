using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class LookupRepository : ILookupRepository
{
    private readonly AppDbContext _context;

    public LookupRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AcademicYear>> GetAcademicYearsAsync()
        => await _context.AcademicYears.AsNoTracking().OrderBy(y => y.Id).ToListAsync();

    public async Task<List<Subject>> GetSubjectsAsync()
        => await _context.Subjects.AsNoTracking().OrderBy(s => s.Id).ToListAsync();

    public async Task<bool> AcademicYearExistsAsync(int id)
        => await _context.AcademicYears.AnyAsync(y => y.Id == id);
}
