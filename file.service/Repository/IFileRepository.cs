using file.service.Models;

namespace file.service.Repository;

public interface IFileRepository
{
    Task<FileMetadata> AddAsync(FileMetadata metadata);
    Task<FileMetadata> GetByIdAsync(string id);
    Task DeleteAsync(string id);
}