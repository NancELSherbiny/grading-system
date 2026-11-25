using Application.DTOs;
using Application.Interfaces;
using DAL.Repositories;

namespace Application.Services;

public class LookupService : ILookupService
{
    private readonly ILookupRepository _lookups;

    public LookupService(ILookupRepository lookups)
    {
        _lookups = lookups;
    }

    public async Task<LookupsDto> GetLookupsAsync()
    {
        var years = await _lookups.GetAcademicYearsAsync();
        var subjects = await _lookups.GetSubjectsAsync();

        return new LookupsDto
        {
            AcademicYears = years.Select(y => new LookupItemDto { Id = y.Id, Name = y.Name }).ToList(),
            Subjects = subjects.Select(s => new LookupItemDto { Id = s.Id, Name = s.Name }).ToList(),
        };
    }
}
