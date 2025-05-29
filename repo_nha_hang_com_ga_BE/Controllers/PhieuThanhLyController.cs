using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.PhieuThanhLy;
using repo_nha_hang_com_ga_BE.Models.Responds.PhieuThanhLy;
using repo_nha_hang_com_ga_BE.Repository;

namespace repo_nha_hang_com_ga_BE.Controllers;

[Authorize]
[ApiController]
[Route("api/phieu-thanh-ly")]
public class PhieuThanhLyController : ControllerBase
{
    private readonly IPhieuThanhLyRepository _repository;

    public PhieuThanhLyController(IPhieuThanhLyRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllPhieuThanhLys([FromQuery] RequestSearchPhieuThanhLy request)
    {
        return Ok(await _repository.GetAllPhieuThanhLys(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPhieuThanhLyById(string id)
    {
        return Ok(await _repository.GetPhieuThanhLyById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreatePhieuThanhLy(RequestAddPhieuThanhLy request)
    {
        return Ok(await _repository.CreatePhieuThanhLy(request));
    }



    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePhieuThanhLy(string id)
    {
        return Ok(await _repository.DeletePhieuThanhLy(id));
    }
}