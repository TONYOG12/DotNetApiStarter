using INFRASTRUCTURE.Context;
using Microsoft.EntityFrameworkCore;
using Moq;
using SHARED.Provider;

namespace TEST.APP.Mocks.Context;

public static class MockApplicationDbContext
{
    public static ApplicationDbContext GetApplicationDbContext()
    {
        var mockTenantProvider = new Mock<ITenantProvider>();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var context = new ApplicationDbContext(options, mockTenantProvider.Object);
        return context;
    }
}