using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Requests.DonDatBan;
using repo_nha_hang_com_ga_BE.Repository;

namespace repo_nha_hang_com_ga_BE.Controllers;

[Authorize]
[ApiController]
[Route("api/don-dat-ban")]

public class DonDatBanController : ControllerBase
{
    private readonly IDonDatBanRepository _repository;

    public DonDatBanController(IDonDatBanRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllDonDatBan([FromQuery] RequestSearchDonDatBan request)
    {
        return Ok(await _repository.GetAllDonDatBan(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDonDatBanById(string id)
    {
        return Ok(await _repository.GetDonDatBanById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateDonDatBan(RequestAddDonDatBan request)
    {
        return Ok(await _repository.CreateDonDatBan(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDonDatBan(string id, RequestUpdateDonDatBan request)
    {
        return Ok(await _repository.UpdateDonDatBan(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDonDatBan(string id)
    {
        return Ok(await _repository.DeleteDonDatBan(id));
    }
}
