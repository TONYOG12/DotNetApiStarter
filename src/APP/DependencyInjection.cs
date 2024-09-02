using APP.IRepository;
using APP.Repository;
using APP.Services.Storage;
using APP.Services.Token;
using Microsoft.Extensions.DependencyInjection;
using SHARED.Provider;
using StackExchange.Redis;

namespace APP;

public static class DependencyInjection
{
    public static void AddTransientServices(this IServiceCollection services)
    {
    }
    
    public static void AddScopedServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        
        services.AddScoped<IBlobStorageService, BlobStorageService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ITenantProvider, TenantProvider>();
    }

    public static void AddSingletonServices(this IServiceCollection services)
    {
        var redisConnectionString = Environment.GetEnvironmentVariable("redisConnectionString") ?? "localhost:6379,abortConnect=false";
        services.AddSingleton<IConnectionMultiplexer>(_ => 
            ConnectionMultiplexer.Connect(redisConnectionString));
        services.AddSingleton(sp => 
            sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
    }
}