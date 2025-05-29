using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.PhieuKiemKe;
using repo_nha_hang_com_ga_BE.Models.Responds.PhieuKiemKe;
using repo_nha_hang_com_ga_BE.Repository;

namespace repo_nha_hang_com_ga_BE.Controllers;

[Authorize]
[ApiController]
[Route("api/phieu-kiem-ke")]
public class PhieuKiemKeController : ControllerBase
{
    private readonly IPhieuKiemKeRepository _repository;

    public PhieuKiemKeController(IPhieuKiemKeRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllPhieuKiemKes([FromQuery] RequestSearchPhieuKiemKe request)
    {
        return Ok(await _repository.GetAllPhieuKiemKes(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPhieuKiemKeById(string id)
    {
        return Ok(await _repository.GetPhieuKiemKeById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreatePhieuKiemKe(RequestAddPhieuKiemKe request)
    {
        return Ok(await _repository.CreatePhieuKiemKe(request));
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePhieuKiemKe(string id)
    {
        return Ok(await _repository.DeletePhieuKiemKe(id));
    }
}