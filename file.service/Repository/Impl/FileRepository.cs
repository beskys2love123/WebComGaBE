using file.service.Models;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using file.service.Context;
using MongoDB.Driver;

namespace file.service.Repository.Impl;

public class FileRepository : IFileRepository
{
    private readonly MongoDbContext _context;

    public FileRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<FileMetadata> AddAsync(FileMetadata metadata)
    {
        await _context.Files.InsertOneAsync(metadata);
        return metadata;
    }

    public async Task<FileMetadata> GetByIdAsync(string id)
    {
        return await _context.Files.Find(f => f.Id == id).FirstOrDefaultAsync();
    }

    public async Task DeleteAsync(string id)
    {
        await _context.Files.DeleteOneAsync(f => f.Id == id);
    }
}