using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.Ban;
using repo_nha_hang_com_ga_BE.Models.Responds.Ban;
using repo_nha_hang_com_ga_BE.Repository;

namespace repo_nha_hang_com_ga_BE.Controllers;

[Authorize]
[ApiController]
[Route("api/ban")]
public class BanController : ControllerBase
{
    private readonly IBanRepository _repository;

    public BanController(IBanRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllBans([FromQuery] RequestSearchBan request)
    {
        return Ok(await _repository.GetAllBans(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBanById(string id)
    {
        return Ok(await _repository.GetBanById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateBan(RequestAddBan request)
    {
        return Ok(await _repository.CreateBan(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBan(string id, RequestUpdateBan request)
    {
        return Ok(await _repository.UpdateBan(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBan(string id)
    {
        return Ok(await _repository.DeleteBan(id));
    }
}