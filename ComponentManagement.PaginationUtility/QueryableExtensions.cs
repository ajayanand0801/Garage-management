using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ComponentManagement.PaginationUtility
{
    public static class QueryableExtensions
    {
        public static IOrderedQueryable<T> ApplySorting<T>(this IQueryable<T> source, System.Collections.Generic.List<SortField> sortFields)
        {
            if (sortFields == null || sortFields.Count == 0)
                return (IOrderedQueryable<T>)source;

            IOrderedQueryable<T>? orderedQuery = null;

            for (int i = 0; i < sortFields.Count; i++)
            {
                var sort = sortFields[i];
                var propInfo = typeof(T).GetProperty(sort.Field,
                    System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (propInfo == null)
                    continue;

                var parameter = Expression.Parameter(typeof(T), "x");
                var member = Expression.Property(parameter, propInfo);
                var keySelector = Expression.Lambda(member, parameter);

                string methodName;
                if (i == 0)
                {
                    methodName = sort.Direction.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
                }
                else
                {
                    methodName = sort.Direction.ToLower() == "desc" ? "ThenByDescending" : "ThenBy";
                }

                var method = typeof(Queryable).GetMethods()
                    .Where(m => m.Name == methodName && m.GetParameters().Length == 2)
                    .Single()
                    .MakeGenericMethod(typeof(T), propInfo.PropertyType);

                if (orderedQuery == null)
                {
                    orderedQuery = (IOrderedQueryable<T>)method.Invoke(null, new object[] { source, keySelector })!;
                }
                else
                {
                    orderedQuery = (IOrderedQueryable<T>)method.Invoke(null, new object[] { orderedQuery, keySelector })!;
                }
            }

            return orderedQuery ?? (IOrderedQueryable<T>)source;
        }
    }

}
