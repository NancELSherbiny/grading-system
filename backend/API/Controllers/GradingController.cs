using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/grading")]
public class GradingController : ControllerBase
{
    private readonly IGradingService _service;

    public GradingController(IGradingService service)
    {
        _service = service;
    }

    // GET api/grading/1?search=&academicYearId=&pageNumber=1&pageSize=10&sortBy=total&sortDirection=desc
    // -> one page of students with their marks in subject 1, plus the succeeded and failed counts
    [HttpGet("{subjectId:int}")]
    public async Task<ActionResult<GradingPageDto>> GetSubject(int subjectId, [FromQuery] GradingQueryDto query)
        => Ok(await _service.GetSubjectAsync(subjectId, query));

    // PUT api/grading/1/students/5   body: { "term1": 25, "term2": 8 }
    // PUT because saving the same marks twice gives the same result (adds the first time, updates after).
    [HttpPut("{subjectId:int}/students/{studentId:int}")]
    public async Task<ActionResult<SubjectMarkDto>> SaveMarks(int subjectId, int studentId, SaveMarksDto dto)
        => Ok(await _service.SaveMarksAsync(subjectId, studentId, dto));
}
