using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.CongThuc;
using repo_nha_hang_com_ga_BE.Models.Responds.CongThuc;
using repo_nha_hang_com_ga_BE.Repository;
using Microsoft.AspNetCore.Authorization;

namespace repo_nha_hang_com_ga_BE.Controllers;

[Authorize]
[ApiController]
[Route("api/cong-thuc")]
public class CongThucController : ControllerBase
{
    private readonly ICongThucRepository _repository;

    public CongThucController(ICongThucRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllCongThucs([FromQuery] RequestSearchCongThuc request)
    {
        return Ok(await _repository.GetAllCongThucs(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCongThucById(string id)
    {
        return Ok(await _repository.GetCongThucById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateCongThuc(RequestAddCongThuc request)
    {
        return Ok(await _repository.CreateCongThuc(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCongThuc(string id, RequestUpdateCongThuc request)
    {
        return Ok(await _repository.UpdateCongThuc(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCongThuc(string id)
    {
        return Ok(await _repository.DeleteCongThuc(id));
    }
}