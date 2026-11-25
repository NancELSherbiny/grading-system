using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/lookups")]
public class LookupsController : ControllerBase
{
    private readonly ILookupService _service;

    public LookupsController(ILookupService service)
    {
        _service = service;
    }

    // GET api/lookups  -> academic years and subjects for the dropdowns
    [HttpGet]
    public async Task<ActionResult<LookupsDto>> Get()
        => Ok(await _service.GetLookupsAsync());
}
