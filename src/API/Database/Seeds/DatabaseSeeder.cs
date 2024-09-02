namespace API.Database.Seeds;

public class DatabaseSeeder(IServiceScope scope)
{
    public void SeedData()
    {
        var items = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
            .Where(x =>
                typeof(ISeeder).IsAssignableFrom(x)
                &&
                !x.IsInterface
                &&
                !x.IsAbstract
            ).ToList();
        foreach (var item in items)
        {
            if (Activator.CreateInstance(item) is ISeeder instance)
                instance.Handle(scope);
        }

    }
}