using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.ThucDon;
using repo_nha_hang_com_ga_BE.Models.Responds.ThucDon;
using repo_nha_hang_com_ga_BE.Repository;
using Microsoft.AspNetCore.Authorization;

namespace repo_nha_hang_com_ga_BE.Controllers;

[ApiController]
[Route("api/thuc-don")]
public class ThucDonController : ControllerBase
{
    private readonly IThucDonRepository _repository;

    public ThucDonController(IThucDonRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllThucDons([FromQuery] RequestSearchThucDon request)
    {
        return Ok(await _repository.GetAllThucDons(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetThucDonById(string id)
    {
        return Ok(await _repository.GetThucDonById(id));
    }

    [Authorize]
    [HttpPost("")]
    public async Task<IActionResult> CreateThucDon(RequestAddThucDon request)
    {
        return Ok(await _repository.CreateThucDon(request));
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateThucDon(string id, RequestUpdateThucDon request)
    {
        return Ok(await _repository.UpdateThucDon(id, request));
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteThucDon(string id)
    {
        return Ok(await _repository.DeleteThucDon(id));
    }
}