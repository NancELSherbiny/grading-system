using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

// [ApiController] checks the DTOs' validation attributes before an action runs,
// and returns 400 with the field errors if any fail.
[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;

    public StudentsController(IStudentService service)
    {
        _service = service;
    }

    // GET api/students?search=ali&academicYearId=1&grade=B&pageNumber=1&pageSize=10&sortBy=name&sortDirection=asc
    [HttpGet]
    public async Task<ActionResult<PagedResult<StudentDto>>> GetAll([FromQuery] StudentQueryDto query)
        => Ok(await _service.GetPagedAsync(query));

    // GET api/students/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<StudentDto>> GetById(int id)
        => Ok(await _service.GetByIdAsync(id));

    // GET api/students/5/grades  -> marks and grade in every subject, plus the overall result
    [HttpGet("{id:int}/grades")]
    public async Task<ActionResult<StudentGradesDto>> GetGrades(int id)
        => Ok(await _service.GetGradesAsync(id));

    // POST api/students  -> 201 Created, with a Location header pointing at the new student
    [HttpPost]
    public async Task<ActionResult<StudentDto>> Create(SaveStudentDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT api/students/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<StudentDto>> Update(int id, SaveStudentDto dto)
        => Ok(await _service.UpdateAsync(id, dto));

    // DELETE api/students/5  -> 204 No Content
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
