using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.LoaiBan;
using repo_nha_hang_com_ga_BE.Models.Responds.LoaiBan;
using repo_nha_hang_com_ga_BE.Repository;
using Microsoft.AspNetCore.Authorization;

namespace repo_nha_hang_com_ga_BE.Controllers;

[Authorize]
[ApiController]
[Route("api/loai-ban")]
public class LoaiBanController : ControllerBase
{
    private readonly ILoaiBanRepository _repository;

    public LoaiBanController(ILoaiBanRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllLoaiBans([FromQuery] RequestSearchLoaiBan request)
    {
        return Ok(await _repository.GetAllLoaiBans(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLoaiBanById(string id)
    {
        return Ok(await _repository.GetLoaiBanById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateLoaiBan(RequestAddLoaiBan request)
    {
        return Ok(await _repository.CreateLoaiBan(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLoaiBan(string id, RequestUpdateLoaiBan request)
    {
        return Ok(await _repository.UpdateLoaiBan(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLoaiBan(string id)
    {
        return Ok(await _repository.DeleteLoaiBan(id));
    }
}