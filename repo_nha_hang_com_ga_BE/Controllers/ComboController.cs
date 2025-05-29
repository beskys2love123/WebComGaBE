using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.Combo;
using repo_nha_hang_com_ga_BE.Models.Responds.Combo;
using repo_nha_hang_com_ga_BE.Repository;
using Microsoft.AspNetCore.Authorization;

namespace repo_nha_hang_com_ga_BE.Controllers;

[ApiController]
[Route("api/combo")]
public class ComboController : ControllerBase
{
    private readonly IComboRepository _repository;

    public ComboController(IComboRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllCombos([FromQuery] RequestSearchCombo request)
    {
        return Ok(await _repository.GetAllCombos(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetComboById(string id)
    {
        return Ok(await _repository.GetComboById(id));
    }

    [Authorize]
    [HttpPost("")]
    public async Task<IActionResult> CreateCombo(RequestAddCombo request)
    {
        return Ok(await _repository.CreateCombo(request));
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCombo(string id, RequestUpdateCombo request)
    {
        return Ok(await _repository.UpdateCombo(id, request));
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCombo(string id)
    {
        return Ok(await _repository.DeleteCombo(id));
    }
}