using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.PhieuXuat;
using repo_nha_hang_com_ga_BE.Models.Responds.PhieuXuat;
using repo_nha_hang_com_ga_BE.Repository;

namespace repo_nha_hang_com_ga_BE.Controllers;

[Authorize]
[ApiController]
[Route("api/phieu-xuat")]
public class PhieuXuatController : ControllerBase
{
    private readonly IPhieuXuatRepository _repository;

    public PhieuXuatController(IPhieuXuatRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllPhieuXuats([FromQuery] RequestSearchPhieuXuat request)
    {
        return Ok(await _repository.GetAllPhieuXuats(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPhieuXuatById(string id)
    {
        return Ok(await _repository.GetPhieuXuatById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreatePhieuXuat(RequestAddPhieuXuat request)
    {
        return Ok(await _repository.CreatePhieuXuat(request));
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePhieuXuat(string id)
    {
        return Ok(await _repository.DeletePhieuXuat(id));
    }
}