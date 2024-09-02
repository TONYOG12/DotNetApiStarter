using Azure.Storage.Blobs.Models;

namespace SHARED.Responses;

public class FileDownloadResponse
{
    public BlobDownloadInfo FileInfo { get; set; }
    public string Name { get; set; }
}