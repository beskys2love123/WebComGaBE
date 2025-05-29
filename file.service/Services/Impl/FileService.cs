using file.service.Context;
using file.service.Models;
using file.service.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace file.service.Services.Impl;

public class FileService : IFileService
{
    private readonly IFileRepository _fileRepository;
    private readonly IWebHostEnvironment _environment;
    private readonly MongoDbContext _context;

    public FileService(
        IFileRepository fileRepository,
        IWebHostEnvironment environment,
        MongoDbContext context)
    {
        _fileRepository = fileRepository;
        _environment = environment;
        _context = context;
    }

    public async Task<FileMetadata> UploadFileAsync(IFormFile file)
    {
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        var fileId = await _context._gridFS.UploadFromStreamAsync(
            file.FileName,
            stream,
            new GridFSUploadOptions
            {
                Metadata = new BsonDocument
                {
                    { "contentType", file.ContentType }
                }
            });

        var metadata = new FileMetadata
        {
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileSize = file.Length,
            FileId = fileId
        };

        await _fileRepository.AddAsync(metadata);
        return metadata;
    }

    public async Task<(byte[] fileBytes, string fileName, string contentType)> DownloadFileAsync(string fileId)
    {
        var metadata = await _fileRepository.GetByIdAsync(fileId);
        if (metadata == null)
            throw new FileNotFoundException("Metadata not found.");

        var stream = new MemoryStream();
        await _context._gridFS.DownloadToStreamAsync(metadata.FileId, stream);
        stream.Position = 0;

        var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", metadata.FileId);
        var fileInfo = (await _context._gridFS.FindAsync(filter)).FirstOrDefault();
        var contentType = fileInfo?.Metadata?.GetValue("contentType", "application/octet-stream").AsString;

        return (stream.ToArray(), metadata.FileName, contentType);
    }

    public async Task DeleteFileAsync(string fileId)
    {
        var metadata = await _fileRepository.GetByIdAsync(fileId);
        if (metadata == null)
            throw new FileNotFoundException("Metadata not found.");

        await _context._gridFS.DeleteAsync(metadata.FileId);
        await _fileRepository.DeleteAsync(fileId);
    }
}