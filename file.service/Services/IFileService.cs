using file.service.Models;

namespace file.service.Services;

public interface IFileService
{
    Task<FileMetadata> UploadFileAsync(IFormFile file);
    Task<(byte[] fileBytes, string fileName, string contentType)> DownloadFileAsync(string fileId);
    Task DeleteFileAsync(string fileId);
}