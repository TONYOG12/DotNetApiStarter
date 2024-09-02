using DOMAIN.Entities.Base;

namespace APP.Extensions;

public static class EntityExtensions
{
    public static bool IsDeleted(this IBaseEntity entity)
    {
        return entity.DeletedAt.HasValue;
    }
}