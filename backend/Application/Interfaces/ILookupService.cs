using Application.DTOs;

namespace Application.Interfaces;

public interface ILookupService
{
    Task<LookupsDto> GetLookupsAsync();
}
