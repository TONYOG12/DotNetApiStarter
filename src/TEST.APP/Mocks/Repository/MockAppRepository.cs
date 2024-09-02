using APP.Repository;
using APP.Services.Storage;
using APP.Services.Token;
using AutoMapper;
using DOMAIN.Entities.Users;
using INFRASTRUCTURE.Context;
using Microsoft.AspNetCore.Identity;

namespace TEST.APP.Mocks.Repository;

public static class MockAppRepository
{
    public static class MockUserRepository
    {
        public static UserRepository GetMockUserRepository(ApplicationDbContext context, IMapper mapper, UserManager<User> manager, 
            IJwtService jwtService, IBlobStorageService storageService)
        {
            return new UserRepository(context,manager, jwtService, storageService, mapper);
        }
    }
}