using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.MonAn;
using repo_nha_hang_com_ga_BE.Models.Responds.MonAn;
using repo_nha_hang_com_ga_BE.Repository;
using Microsoft.AspNetCore.Authorization;

namespace repo_nha_hang_com_ga_BE.Controllers;

[ApiController]
[Route("api/mon-an")]
public class MonAnController : ControllerBase
{
    private readonly IMonAnRepository _repository;

    public MonAnController(IMonAnRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllMonAns([FromQuery] RequestSearchMonAn request)
    {
        return Ok(await _repository.GetAllMonAns(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMonAnById(string id)
    {
        return Ok(await _repository.GetMonAnById(id));
    }

    [Authorize]
    [HttpPost("")]
    public async Task<IActionResult> CreateMonAn(RequestAddMonAn request)
    {
        return Ok(await _repository.CreateMonAn(request));
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMonAn(string id, RequestUpdateMonAn request)
    {
        return Ok(await _repository.UpdateMonAn(id, request));
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMonAn(string id)
    {
        return Ok(await _repository.DeleteMonAn(id));
    }
}