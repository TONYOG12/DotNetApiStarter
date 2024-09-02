using System.Linq.Expressions;
using LinqKit;

namespace APP.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> WhereSearch<T>(this IQueryable<T> source, string searchQuery, params Expression<Func<T, string>>[] properties)
    {
        if (string.IsNullOrEmpty(searchQuery))
        {
            return source; 
        }

        searchQuery = searchQuery.ToLower();
        var predicate = PredicateBuilder.New<T>(false); 

        foreach (var property in properties)
        {
            var lowerCaseProperty = Expression.Lambda<Func<T, bool>>(
                Expression.Call(
                    Expression.Call(property.Body, "ToLower", null),
                    "Contains",
                    null,
                    Expression.Constant(searchQuery)
                ),
                property.Parameters
            );

            predicate = predicate.Or(lowerCaseProperty);
        }

        return source.Where(predicate);
    }
}