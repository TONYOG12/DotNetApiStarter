using APP.Repository;
using APP.Utils;
using AutoMapper;
using DOMAIN.Entities.Roles;
using DOMAIN.Entities.Users;
using FluentAssertions;
using INFRASTRUCTURE.Context;
using Microsoft.EntityFrameworkCore;
using Moq;
using TEST.APP.Mocks.Context;
using TEST.APP.Mocks.Mapper;
using TEST.APP.Mocks.Repository;
using Xunit;

namespace TEST.APP.UnitTests;

public class UserTest
{
    private readonly ApplicationDbContext _context; 
    private readonly UserRepository _repository; 
    private readonly Mock<IMapper> _mockMapper;
    
    public UserTest() 
    { 
        // Initialize in-memory database context
        _context = MockApplicationDbContext.GetApplicationDbContext();

        // Initialize mocks
        _mockMapper = MockMapper.GetMockMapper(); 
        var mockUserManager = MockUserManager.GetMockUserManager();
        var mockBlobStorageService = MockAppService.GetMockBlobStorageService(); 
        var mockJwtService = MockAppService.GetMockJwtService();

        // Seed roles before testing users
        SeedRoles(_context);

            // Initialize repository
            _repository = MockAppRepository.MockUserRepository.GetMockUserRepository(
                _context, _mockMapper.Object, mockUserManager.Object, mockJwtService.Object, mockBlobStorageService.Object);
    }
    
    private static void SeedRoles(ApplicationDbContext dbContext)
    {
        foreach (var roleName in RoleUtils.AppRoles())
        {
            var role = dbContext.Roles.IgnoreQueryFilters().FirstOrDefault(item => item.Name == roleName);
            if (role == null)
            {
                var newRole = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpper(),
                    DisplayName = roleName.Replace(".", "").ToUpper()
                };

                dbContext.Roles.Add(newRole);
            }
        }

        dbContext.SaveChanges();
    }

    [Fact]
    public void ValidateRoles_SeededSuccessfully()
    {
        // Act
        var roles = _context.Roles.ToList();

        // Assert
        roles.Should().NotBeEmpty();
        roles.Should().Contain(r => r.Name == RoleUtils.AppRoleSuper);
        roles.Should().Contain(r => RoleUtils.AppRoles().Contains(r.Name));
    }

    [Fact]
    public async Task CreateUser_ShouldReturnSuccess_WhenValidRequestIsProvided()
    {
        // Arrange
        var createUserRequest = new CreateUserRequest
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Password = "Password123!",
            RoleNames = ["admin"]
        };

        // Act
        var result = await _repository.CreateUser(createUserRequest);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
        _mockMapper.Verify(m => m.Map<User>(createUserRequest), Times.Once);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnError_WhenEmailIsNotUnique()
    {
        // Arrange
        var createUserRequest = new CreateUserRequest
        {
            Email = "duplicate@example.com",
            FirstName = "John",
            LastName = "Doe",
            Password = "Password123!",
            RoleNames = ["admin"]
        };

        var existingUser = new User
        {
            Email = createUserRequest.Email
        };

        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.CreateUser(createUserRequest);

        // Assert
        result.IsSuccess.Should().BeFalse();
        Assert.Equal(UserErrors.EmailNotUnique, result.Error);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnSuccess_WhenUserIsUpdatedSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updateUserRequest = new UpdateUserRequest { FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com" };
        var userEntity = new User { Id = userId, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };

        _context.Users.Add(userEntity);
        await _context.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map(updateUserRequest, userEntity));

        // Act
        var result = await _repository.UpdateUser(updateUserRequest, userId, userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var updatedUser = await _repository.GetUser(userId);
        updatedUser.Value.FirstName.Should().Be("Jane");
        updatedUser.Value.LastName.Should().Be("Doe");
        updatedUser.Value.Email.Should().Be("jane.doe@example.com");
    }

    [Fact] 
    public async Task DeleteUser_ShouldReturnSuccess_WhenUserIsDeletedSuccessfully() 
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userEntity = new User { Id = userId, Email = "john.doe@example.com" };

        _context.Users.Add(userEntity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteUser(userId, userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        userEntity.DeletedAt.Should().NotBeNull();
    }
        
    //add more tests to account for edge cases
}
    