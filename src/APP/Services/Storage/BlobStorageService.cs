using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using SHARED;
using SHARED.Responses;

namespace APP.Services.Storage;

public class BlobStorageService : IBlobStorageService
{
    private readonly string _accessKey = Environment.GetEnvironmentVariable("MINIO_ACCESS_KEY");
    private readonly string _secretKey = Environment.GetEnvironmentVariable("MINIO_SECRET_KEY");
    private readonly string _endpoint = Environment.GetEnvironmentVariable("MINIO_ENDPOINT");
    private readonly string _port = Environment.GetEnvironmentVariable("MINIO_PORT");

    public async Task<Result> UploadBlobAsync(string bucketName, IFormFile file, string objectName,
        string previousObjectName = null)
    {
        try
        {
            if (!int.TryParse(_port, out var port))
            {
                return StorageErrors.PortNotFound(nameof(port));
            }

            var minioClient = new MinioClient()
                .WithEndpoint(_endpoint, port)
                .WithCredentials(_accessKey, _secretKey)
                .Build();

            // Create the bucket if it doesn't exist
            if (!await minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName)))
            {
                await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
            }

            if (previousObjectName != null)
            {
                await minioClient.RemoveObjectAsync(new RemoveObjectArgs().WithBucket(bucketName)
                    .WithObject(previousObjectName));
            }

            await using var stream = file.OpenReadStream();
            await minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(file.ContentType));
            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(StorageErrors.SaveFileFailure(e.Message));
        }
    }

    public Result UploadBlob(string bucketName, IFormFile file, string objectName, string previousObjectName = null)
    {
        try
        {
            if (!int.TryParse(_port, out var port))
            {
                return StorageErrors.PortNotFound(nameof(port));
            }

            var minioClient = new MinioClient()
                .WithEndpoint(_endpoint, port)
                .WithCredentials(_accessKey, _secretKey)
                .Build();

            // Create the bucket if it doesn't exist
            if (!minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName)).Result)
            {
                minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName)).Wait();
            }

            if (previousObjectName != null)
            {
                minioClient.RemoveObjectAsync(new RemoveObjectArgs().WithBucket(bucketName)
                    .WithObject(previousObjectName)).Wait();
            }

            using var stream = file.OpenReadStream();
            minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(file.ContentType)).Wait();

            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(StorageErrors.SaveFileFailure(e.Message));
        }
    }

    public async Task<Result<(Stream Stream, string ContentType, string Name)>> GetBlobAsync(string bucketName,
        string objectName)
    {
        if (!int.TryParse(_port, out var port))
        {
            return StorageErrors.PortNotFound(nameof(port));
        }

        var minioClient = new MinioClient()
            .WithEndpoint(_endpoint, port)
            .WithCredentials(_accessKey, _secretKey)
            .Build();

        try
        {
            var stream = new MemoryStream();
            var getObjectArgs = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithCallbackStream((objectStream) => objectStream.CopyTo(stream));

            await minioClient.GetObjectAsync(getObjectArgs);
            stream.Seek(0, SeekOrigin.Begin);

            // Retrieve object metadata to get the content type if needed
            var statObjectArgs = new StatObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName);
            var metadata = await minioClient.StatObjectAsync(statObjectArgs);

            return (stream, metadata.ContentType, objectName);
        }
        catch (Exception)
        {
            return Error.Failure("Download.Failure", "Unable to download image");
        }
    }

    public async Task<Result<(Stream Stream, string ContentType, string Name)>> GetBlobAsync(string bucketName,
        string modelId, string reference)
    {
        var objectName = $"{modelId}/{reference}";
        return await GetBlobAsync(bucketName, objectName);
    }
    
    public async Task<Result> UploadChunkAsync(string bucketName, IFormFile chunk, string objectName, int chunkIndex)
    {
        try
        {
            if (!int.TryParse(_port, out var port))
            {
                return StorageErrors.PortNotFound(nameof(port));
            }
            
            var minioClient = new MinioClient()
                .WithEndpoint(_endpoint, port)
                .WithCredentials(_accessKey, _secretKey)
                .Build();

            // Create the bucket if it doesn't exist
            bool bucketExists = await minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
            if (!bucketExists)
            {
                await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
            }

            var tempObjectName = $"{objectName}.part{chunkIndex}";
            await using var stream = chunk.OpenReadStream();
            await minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(tempObjectName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(chunk.ContentType));

            return Result.Success();
        }
        catch (MinioException e)
        {
            return Result.Failure(StorageErrors.SaveFileFailure(e.Message));
        }
    }

    public async Task<Result> CombineChunksAsync(string bucketName, string objectName, int totalChunks)
    {
        try
        {
            if (!int.TryParse(_port, out var port))
            {
                return StorageErrors.PortNotFound(nameof(port));
            }
            
            var minioClient = new MinioClient()
                .WithEndpoint(_endpoint, port)
                .WithCredentials(_accessKey, _secretKey)
                .Build();

            using (var combinedStream = new MemoryStream())
            {
                for (int i = 0; i < totalChunks; i++)
                {
                    var tempObjectName = $"{objectName}.part{i}";
                    var tempStream = new MemoryStream();
                    await minioClient.GetObjectAsync(new GetObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(tempObjectName)
                        .WithCallbackStream(tempStream.CopyTo));

                    tempStream.Seek(0, SeekOrigin.Begin);
                    await tempStream.CopyToAsync(combinedStream);
                }

                combinedStream.Seek(0, SeekOrigin.Begin);
                await minioClient.PutObjectAsync(new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithStreamData(combinedStream)
                    .WithObjectSize(combinedStream.Length)
                    .WithContentType("application/octet-stream"));

                // Cleanup temporary parts
                for (int i = 0; i < totalChunks; i++)
                {
                    var tempObjectName = $"{objectName}.part{i}";
                    await minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(tempObjectName));
                }
            }

            return Result.Success();
        }
        catch (MinioException e)
        {
            return Result.Failure(StorageErrors.SaveFileFailure(e.Message));
        }
    }
}