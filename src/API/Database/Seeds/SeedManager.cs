using Microsoft.EntityFrameworkCore;
using INFRASTRUCTURE.Context;

namespace API.Database.Seeds;

public static class SeedManager
{
    public static IHost SeedData(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        try
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
        }
        catch (Exception)
        {
            // ignored
        }

        try
        {
            var databaseSeeder = new DatabaseSeeder(scope);
            databaseSeeder.SeedData();
        }
        catch (Exception)
        {
            // ignored
        }
        return host;
    }
}