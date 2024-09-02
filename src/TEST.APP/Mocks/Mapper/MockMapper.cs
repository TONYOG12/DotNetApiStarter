using AutoMapper;
using DOMAIN.Entities.Roles;
using DOMAIN.Entities.Users;
using Moq;

namespace TEST.APP.Mocks.Mapper;

public static class MockMapper
{
    public static Mock<IMapper> GetMockMapper()
    {
        var mockMapper = new Mock<IMapper>();

        #region UserMapper

        // Map CreateUserRequest to User
        mockMapper.Setup(m => m.Map<User>(It.IsAny<CreateUserRequest>()))
            .Returns((CreateUserRequest request) => new User
            {
                Id = Guid.NewGuid(),
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                // Add additional mappings as needed
            });

        // Map User to UserDto
        mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
            .Returns((User user) => new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                // Example Roles - in a real test, these would be mapped based on the actual user roles
                Roles = new List<RoleDto>
                {
                    new RoleDto { Id = Guid.NewGuid(), Name = "Role1" },
                    new RoleDto { Id = Guid.NewGuid(), Name = "Role2" }
                },
                Avatar = "https://example.com/avatar.jpg" // Example Avatar URL
            });


        #endregion

        #region RoleMapper

        // Map CreateRoleRequest to Role
        mockMapper.Setup(m => m.Map<Role>(It.IsAny<CreateRoleRequest>()))
            .Returns((CreateRoleRequest request) => new Role
            {
                Id = Guid.NewGuid(),
                Name = request.Name
            });

        // Map Role to RoleDto
        mockMapper.Setup(m => m.Map<RoleDto>(It.IsAny<Role>()))
            .Returns((Role role) => new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                DisplayName = role.DisplayName
            });

        // Reverse Map RoleDto to Role
        mockMapper.Setup(m => m.Map<Role>(It.IsAny<RoleDto>()))
            .Returns((RoleDto roleDto) => new Role
            {
                Id = roleDto.Id,
                Name = roleDto.Name,
                DisplayName = roleDto.DisplayName
            });

        #endregion

        return mockMapper;
    }
}