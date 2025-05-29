﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.NhaHang;
using repo_nha_hang_com_ga_BE.Models.Responds.NhaHang;
using repo_nha_hang_com_ga_BE.Repository;

namespace repo_nha_hang_com_ga_BE.Controllers;

[ApiController]
[Route("api/nha-hang")]
public class NhaHangController : ControllerBase
{
    private readonly INhaHangRepository _repository;

    public NhaHangController(INhaHangRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllNhaHangs([FromQuery] RequestSearchNhaHang request)
    {
        return Ok(await _repository.GetAllNhaHangs(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNhaHangById(string id)
    {
        return Ok(await _repository.GetNhaHangById(id));
    }

    [Authorize]
    [HttpPost("")]
    public async Task<IActionResult> CreateNhaHang(RequestAddNhaHang request)
    {
        return Ok(await _repository.CreateNhaHang(request));
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNhaHang(string id, RequestUpdateNhaHang request)
    {
        return Ok(await _repository.UpdateNhaHang(id, request));
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNhaHang(string id)
    {
        return Ok(await _repository.DeleteNhaHang(id));
    }
}