using CandidateManagement.Application.DTOs;
using CandidateManagement.Application.Services;
using CandidateManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CandidateManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CandidatesController(ICandidateService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Candidate>> AddOrUpdate([FromBody] CandidateDto dto)
    {
        var candidate = await service.AddOrUpdateAsync(dto);
        return Ok(candidate);
    }
}
