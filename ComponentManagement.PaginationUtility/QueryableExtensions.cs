using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ComponentManagement.PaginationUtility
{
    public static class QueryableExtensions
    {
        private static readonly BindingFlags PropertyFlags = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        public static IOrderedQueryable<T> ApplySorting<T>(this IQueryable<T> source, System.Collections.Generic.List<SortField> sortFields)
        {
            if (sortFields == null || sortFields.Count == 0)
                return (IOrderedQueryable<T>)source;

            IOrderedQueryable<T>? orderedQuery = null;
            var parameter = Expression.Parameter(typeof(T), "x");

            for (int i = 0; i < sortFields.Count; i++)
            {
                var sort = sortFields[i];
                if (string.IsNullOrWhiteSpace(sort.Field))
                    continue;

                Expression? member;
                Type memberType;

                if (sort.Field.Contains("."))
                {
                    var pathResult = BuildNestedMemberExpression(typeof(T), parameter, sort.Field);
                    if (pathResult == null)
                        continue;
                    member = pathResult.Value.Member;
                    memberType = pathResult.Value.MemberType;
                }
                else
                {
                    var propInfo = typeof(T).GetProperty(sort.Field, PropertyFlags);
                    if (propInfo == null)
                        continue;
                    member = Expression.Property(parameter, propInfo);
                    memberType = propInfo.PropertyType;
                }

                var keySelector = Expression.Lambda(member, parameter);

                string methodName = sort.Direction?.ToLower() == "desc"
                    ? (i == 0 ? "OrderByDescending" : "ThenByDescending")
                    : (i == 0 ? "OrderBy" : "ThenBy");

                var method = typeof(Queryable).GetMethods()
                    .Where(m => m.Name == methodName && m.GetParameters().Length == 2)
                    .Single()
                    .MakeGenericMethod(typeof(T), memberType);

                if (orderedQuery == null)
                    orderedQuery = (IOrderedQueryable<T>)method.Invoke(null, new object[] { source, keySelector })!;
                else
                    orderedQuery = (IOrderedQueryable<T>)method.Invoke(null, new object[] { orderedQuery, keySelector })!;
            }

            return orderedQuery ?? (IOrderedQueryable<T>)source;
        }

        private static (Expression Member, Type MemberType)? BuildNestedMemberExpression(Type rootType, ParameterExpression parameter, string path)
        {
            var parts = path.Split('.');
            Expression current = parameter;
            Type currentType = rootType;

            foreach (var part in parts)
            {
                if (string.IsNullOrWhiteSpace(part))
                    return null;
                var prop = currentType.GetProperty(part.Trim(), PropertyFlags);
                if (prop == null)
                    return null;
                current = Expression.Property(current, prop);
                currentType = prop.PropertyType;
            }

            return (current, currentType);
        }
    }
}
