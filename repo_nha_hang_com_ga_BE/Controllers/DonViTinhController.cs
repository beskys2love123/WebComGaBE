using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.DonViTinh;
using repo_nha_hang_com_ga_BE.Models.Responds.DonViTinh;
using repo_nha_hang_com_ga_BE.Repository;
using Microsoft.AspNetCore.Authorization;

namespace repo_nha_hang_com_ga_BE.Controllers;

[Authorize]
[ApiController]
[Route("api/don-vi-tinh")]
public class DonViTinhController : ControllerBase
{
    private readonly IDonViTinhRepository _repository;

    public DonViTinhController(IDonViTinhRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllDonViTinhs([FromQuery] RequestSearchDonViTinh request)
    {
        return Ok(await _repository.GetAllDonViTinhs(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDonViTinhById(string id)
    {
        return Ok(await _repository.GetDonViTinhById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateDonViTinh(RequestAddDonViTinh request)
    {
        return Ok(await _repository.CreateDonViTinh(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDonViTinh(string id, RequestUpdateDonViTinh request)
    {
        return Ok(await _repository.UpdateDonViTinh(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDonViTinh(string id)
    {
        return Ok(await _repository.DeleteDonViTinh(id));
    }
}