using APP.Services.Storage;
using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Http;
using TEST.APP.Mocks.Repository;

namespace TEST.APP.UnitTests;


public class BlobStorageServiceTests
{
    private readonly Mock<IBlobStorageService> _mockBlobStorageService = MockAppService.GetMockBlobStorageService();

    [Fact]
    public async Task UploadBlobAsync_ShouldReturnSuccess_WhenUploadIsSuccessful()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());
        mockFile.Setup(f => f.ContentType).Returns("text/plain");

        // Act
        var result = await _mockBlobStorageService.Object.UploadBlobAsync("bucket-name", mockFile.Object, "object-name");

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GetBlobAsync_ShouldReturnSuccess_WhenFileExists()
    {
        // Act
        var result = await _mockBlobStorageService.Object.GetBlobAsync("bucket-name", "object-name");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Stream.Should().NotBeNull();
        result.Value.ContentType.Should().Be("application/octet-stream");
    }

    [Fact]
    public async Task UploadChunkAsync_ShouldReturnSuccess_WhenChunkUploadIsSuccessful()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());
        mockFile.Setup(f => f.ContentType).Returns("text/plain");

        // Act
        var result = await _mockBlobStorageService.Object.UploadChunkAsync("bucket-name", mockFile.Object, "object-name", 1);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task CombineChunksAsync_ShouldReturnSuccess_WhenChunksAreCombinedSuccessfully()
    {
        // Act
        var result = await _mockBlobStorageService.Object.CombineChunksAsync("bucket-name", "object-name", 2);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
