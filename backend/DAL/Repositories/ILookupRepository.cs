using DAL.Models;

namespace DAL.Repositories;

public interface ILookupRepository
{
    Task<List<AcademicYear>> GetAcademicYearsAsync();

    Task<List<Subject>> GetSubjectsAsync();

    Task<bool> AcademicYearExistsAsync(int id);
}
