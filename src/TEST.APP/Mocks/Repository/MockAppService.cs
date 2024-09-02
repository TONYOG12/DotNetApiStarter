using APP.Services.Storage;
using APP.Services.Token;
using DOMAIN.Entities.Auth;
using DOMAIN.Entities.Users;
using Microsoft.AspNetCore.Http;
using Moq;
using SHARED;

namespace TEST.APP.Mocks.Repository;

public static class MockAppService
{
    public static Mock<IJwtService> GetMockJwtService()
    {
        var mockJwtService = new Mock<IJwtService>();

        // Setup methods and properties of the JwtService as needed
        mockJwtService.Setup(j => j.AuthenticateNewUser(It.IsAny<User>()))
            .ReturnsAsync(new LoginResponse 
            { 
                AccessToken = "mock_token", 
                UserId = Guid.NewGuid(),
                ExpiresIn = Convert.ToInt32(DateTime.Now.AddMonths(1).Subtract(DateTime.UtcNow).TotalSeconds),
            });

        return mockJwtService;
    }
    
    public static Mock<IBlobStorageService> GetMockBlobStorageService()
    {
        var mockBlobStorageService = new Mock<IBlobStorageService>();

        // Mock UploadBlobAsync
        mockBlobStorageService.Setup(s => s.UploadBlobAsync(It.IsAny<string>(), It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Result.Success());

        // Mock UploadBlob
        mockBlobStorageService.Setup(s => s.UploadBlob(It.IsAny<string>(), It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Result.Success());

        // Mock GetBlobAsync (by containerName and objectName)
        mockBlobStorageService.Setup(s => s.GetBlobAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Result.Success<(Stream, string, string)>((
                new MemoryStream(), 
                "application/octet-stream", 
                "test.txt")));

        // Mock GetBlobAsync (by containerName, modelId, reference)
        mockBlobStorageService.Setup(s => s.GetBlobAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Result.Success<(Stream, string, string)>((
                new MemoryStream(), 
                "application/octet-stream", 
                "test.txt")));

        // Mock UploadChunkAsync
        mockBlobStorageService.Setup(s => s.UploadChunkAsync(It.IsAny<string>(), It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Success());

        // Mock CombineChunksAsync
        mockBlobStorageService.Setup(s => s.CombineChunksAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Success());

        return mockBlobStorageService;
    }
}