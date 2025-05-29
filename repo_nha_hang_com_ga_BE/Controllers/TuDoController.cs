using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.TuDo;
using repo_nha_hang_com_ga_BE.Models.Responds.TuDo;
using repo_nha_hang_com_ga_BE.Repository;
using Microsoft.AspNetCore.Authorization;

namespace repo_nha_hang_com_ga_BE.Controllers;

[Authorize]
[ApiController]
[Route("api/tu-do")]
public class TuDoController : ControllerBase
{
    private readonly ITuDoRepository _repository;

    public TuDoController(ITuDoRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllTuDos([FromQuery] RequestSearchTuDo request)
    {
        return Ok(await _repository.GetAllTuDos(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTuDoById(string id)
    {
        return Ok(await _repository.GetTuDoById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateTuDo(RequestAddTuDo request)
    {
        return Ok(await _repository.CreateTuDo(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTuDo(string id, RequestUpdateTuDo request)
    {
        return Ok(await _repository.UpdateTuDo(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTuDo(string id)
    {
        return Ok(await _repository.DeleteTuDo(id));
    }
}