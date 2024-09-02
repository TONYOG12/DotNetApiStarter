using AutoMapper;
using DOMAIN.Entities.Roles;
using DOMAIN.Entities.Users;
using INFRASTRUCTURE.Context;

namespace APP.Mapper.Resolvers;

public class UserRoleResolver(ApplicationDbContext context, IMapper mapper)
    : IValueResolver<User, UserDto, ICollection<RoleDto>>
{
    public ICollection<RoleDto> Resolve(User source, UserDto destination, ICollection<RoleDto> destMember,
        ResolutionContext context1)
    {
        var roleIds = context
            .UserRoles
            .Where(item => item.UserId == source.Id)
            .Select(user => user.RoleId)
            .ToList();
        return context.Roles.Where(role => roleIds.Contains(role.Id)).Select(role => mapper.Map<RoleDto>(role))
            .ToList();
    }
}