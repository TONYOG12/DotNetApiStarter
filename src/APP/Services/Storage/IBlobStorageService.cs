using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using SHARED;
using SHARED.Responses;

namespace APP.Services.Storage;

public interface IBlobStorageService
{
    Task<Result> UploadBlobAsync(string bucketName, IFormFile file, string objectName, string previousObjectName = null);
    Result UploadBlob(string bucketName, IFormFile file, string objectName, string previousObjectName = null);
    Task<Result<(Stream Stream, string ContentType, string Name)>> GetBlobAsync(string bucketName, string objectName);
    Task<Result<(Stream Stream, string ContentType, string Name)>> GetBlobAsync(string bucketName,
        string modelId, string reference);
    Task<Result> UploadChunkAsync(string bucketName, IFormFile chunk, string objectName, int chunkIndex);
    Task<Result> CombineChunksAsync(string bucketName, string objectName, int totalChunks);
}