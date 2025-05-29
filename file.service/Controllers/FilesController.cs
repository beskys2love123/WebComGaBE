using file.service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace file.service.Controllers;

[ApiController]
[Route("api/files")]
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;

    public FilesController(IFileService fileService) => _fileService = fileService;

    [Authorize]
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var metadata = await _fileService.UploadFileAsync(file);
        return Ok(metadata);
    }

    [HttpGet("download/{fileId}")]
    public async Task<IActionResult> Download(string fileId)
    {
        var (fileBytes, fileName, contentType) = await _fileService.DownloadFileAsync(fileId);
        return File(fileBytes, contentType, fileName);
    }

    [Authorize]
    [HttpDelete("delete/{fileId}")]
    public async Task<IActionResult> Delete(string fileId)
    {
        await _fileService.DeleteFileAsync(fileId);
        return NoContent();
    }
}