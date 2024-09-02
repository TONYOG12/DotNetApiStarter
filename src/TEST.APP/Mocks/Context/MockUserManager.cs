using DOMAIN.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace TEST.APP.Mocks.Context;

public static class MockUserManager
{
    public static Mock<UserManager<User>> GetMockUserManager()
    {
        var store = new Mock<IUserStore<User>>();
    
        // Providing necessary dependencies for the UserManager constructor
        var mockOptions = new Mock<IOptions<IdentityOptions>>();
        var mockPasswordHasher = new Mock<IPasswordHasher<User>>();
        var mockUserValidators = new List<IUserValidator<User>> { new Mock<IUserValidator<User>>().Object };
        var mockPasswordValidators = new List<IPasswordValidator<User>> { new Mock<IPasswordValidator<User>>().Object };
        var mockLookupNormalizer = new Mock<ILookupNormalizer>();
        var mockIdentityErrorDescriber = new Mock<IdentityErrorDescriber>();
        var mockServices = new Mock<IServiceProvider>();
        var mockLogger = new Mock<ILogger<UserManager<User>>>();

        var mockUserManager = new Mock<UserManager<User>>(
            store.Object,
            mockOptions.Object,
            mockPasswordHasher.Object,
            mockUserValidators,
            mockPasswordValidators,
            mockLookupNormalizer.Object,
            mockIdentityErrorDescriber.Object,
            mockServices.Object,
            mockLogger.Object
        );

        mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        mockUserManager.Setup(x => x.AddToRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Success);

        mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<User>()))
            .ReturnsAsync(new List<string>());

        mockUserManager.Setup(x => x.RemoveFromRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Success);

        mockUserManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()))
            .ReturnsAsync("mock_token");

        mockUserManager.Setup(x => x.ConfirmEmailAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        return mockUserManager;
    }
    
    /*public static Mock<RoleManager<Role>> GetMockRoleManager()
    {
        var mockRoleStore = new Mock<IRoleStore<Role>>();

        var mockRoleManager = new Mock<RoleManager<Role>>(
            mockRoleStore.Object);

        // Setup default behavior for CreateAsync
        mockRoleManager.Setup(r => r.CreateAsync(It.IsAny<Role>()))
            .ReturnsAsync(IdentityResult.Success);

        // Setup default behavior for RoleExistsAsync
        mockRoleManager.Setup(r => r.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Setup default behavior for FindByNameAsync
        mockRoleManager.Setup(r => r.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((string roleName) => new Role { Name = roleName });

        // Add more setups as needed for your tests
        return mockRoleManager;
    }*/
}
