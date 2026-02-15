using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace ComponentManagement.PaginationUtility
{
    public static class ExpressionBuilder
    {
        public static Expression<Func<T, bool>> BuildPredicate<T>(List<FilterField> filters)
        {
            if (filters == null || filters.Count == 0)
                return x => true;

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? body = null;

            foreach (var filter in filters)
            {
                var expr = BuildSinglePredicate<T>(parameter, filter);
                if (expr == null)
                    continue;

                if (body == null)
                    body = expr;
                else
                {
                    if (filter.LogicalOperator?.ToLowerInvariant() == "or")
                        body = Expression.OrElse(body, expr);
                    else
                        body = Expression.AndAlso(body, expr);
                }
            }

            if (body == null)
                return x => true;

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        private static Expression? BuildSinglePredicate<T>(ParameterExpression parameter, FilterField filter)
        {
            var flags = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

            // Nested path: "parent.child" (single navigation) or "parentCollection.child" (collection Any)
            if (!string.IsNullOrWhiteSpace(filter.Field) && filter.Field.Contains("."))
            {
                var parts = filter.Field.Split('.', 2);
                var parentPropName = parts[0];
                var nestedPropName = parts[1];

                var parentProp = typeof(T).GetProperty(parentPropName, flags);
                if (parentProp == null)
                    return null;

                var parentType = parentProp.PropertyType;
                var isCollection = parentType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(parentType);

                if (isCollection)
                {
                    // Collection: e.g. vehicleMetaData.Make -> Any(v => v.Make == value)
                    Type? elementType = null;
                    if (parentType.IsGenericType)
                        elementType = parentType.GetGenericArguments()[0];
                    else
                    {
                        var iface = parentType.GetInterfaces()
                            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                        if (iface != null)
                            elementType = iface.GetGenericArguments()[0];
                    }
                    if (elementType == null)
                        return null;

                    var nestedPropInfo = elementType.GetProperty(nestedPropName, flags);
                    if (nestedPropInfo == null)
                        return null;

                    var innerParam = Expression.Parameter(elementType, "c");
                    var nestedMember = Expression.Property(innerParam, nestedPropInfo);

                    var nonNullableNestedType = Nullable.GetUnderlyingType(nestedPropInfo.PropertyType) ?? nestedPropInfo.PropertyType;
                    var typedValue = ConvertFilterValue(filter.Value, nonNullableNestedType);
                    if (typedValue == null)
                        return null;

                    var constant = Expression.Constant(typedValue, nonNullableNestedType);
                    Expression nestedMemberExpr = nestedMember;
                    if (Nullable.GetUnderlyingType(nestedPropInfo.PropertyType) != null)
                        nestedMemberExpr = Expression.Convert(nestedMember, nonNullableNestedType);

                    var operation = NormalizeOperation(filter.Operation);
                    Expression? innerExpr = operation switch
                    {
                        "eq" => Expression.Equal(nestedMemberExpr, constant),
                        "ne" => Expression.NotEqual(nestedMemberExpr, constant),
                        "contains" => BuildStringContains(nestedMember, constant),
                        _ => null
                    };
                    if (innerExpr == null)
                        return null;

                    var lambdaInner = Expression.Lambda(innerExpr, innerParam);
                    var collectionMember = Expression.Property(parameter, parentProp);
                    var anyMethod = typeof(Enumerable)
                        .GetMethods(BindingFlags.Static | BindingFlags.Public)
                        .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
                        .MakeGenericMethod(elementType);
                    return Expression.Call(anyMethod, collectionMember, lambdaInner);
                }
                else
                {
                    // Single navigation: e.g. customerMetaData.Email -> customerMetaData != null && customerMetaData.Email == value
                    var nestedPropInfo = parentType.GetProperty(nestedPropName, flags);
                    if (nestedPropInfo == null)
                        return null;

                    var parentMember = Expression.Property(parameter, parentProp);
                    var childMember = Expression.Property(parentMember, nestedPropInfo);

                    var propType = nestedPropInfo.PropertyType;
                    var nonNullableType = Nullable.GetUnderlyingType(propType) ?? propType;
                    var typedValue = ConvertFilterValue(filter.Value, nonNullableType);
                    if (typedValue == null)
                        return null;

                    var constant = Expression.Constant(typedValue, nonNullableType);
                    Expression childExpr = childMember;
                    if (Nullable.GetUnderlyingType(propType) != null)
                        childExpr = Expression.Convert(childMember, nonNullableType);

                    var operation = NormalizeOperation(filter.Operation);
                    Expression? comparison = operation switch
                    {
                        "eq" => Expression.Equal(childExpr, constant),
                        "ne" => Expression.NotEqual(childExpr, constant),
                        "contains" => BuildStringContains(childMember, constant),
                        _ => null
                    };
                    if (comparison == null)
                        return null;

                    var parentNotNull = Expression.NotEqual(parentMember, Expression.Constant(null, parentType));
                    return Expression.AndAlso(parentNotNull, comparison);
                }
            }
            else
            {
                // Simple (non-nested) property
                var propInfo = typeof(T).GetProperty(filter.Field, flags);

                if (propInfo == null)
                    return null;

                var member = Expression.Property(parameter, propInfo);
                var propType = propInfo.PropertyType;
                var nonNullableType = Nullable.GetUnderlyingType(propType) ?? propType;

                var typedValue = ConvertFilterValue(filter.Value, nonNullableType);
                if (typedValue == null)
                    return null;

                var constant = Expression.Constant(typedValue, nonNullableType);
                Expression memberExpr = member;
                if (Nullable.GetUnderlyingType(propType) != null)
                    memberExpr = Expression.Convert(member, nonNullableType);

                var operation = NormalizeOperation(filter.Operation);
                switch (operation)
                {
                    case "eq":
                        return Expression.Equal(memberExpr, constant);
                    case "ne":
                        return Expression.NotEqual(memberExpr, constant);
                    case "gt":
                        return Expression.GreaterThan(memberExpr, constant);
                    case "lt":
                        return Expression.LessThan(memberExpr, constant);
                    case "gte":
                        return Expression.GreaterThanOrEqual(memberExpr, constant);
                    case "lte":
                        return Expression.LessThanOrEqual(memberExpr, constant);
                    case "contains":
                        if (nonNullableType == typeof(string))
                            return BuildStringContains(member, constant);
                        break;
                    default:
                        return null;
                }

                return null;
            }
        }

        private static Expression? BuildStringContains(Expression stringMember, ConstantExpression constant)
        {
            var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            return method != null ? Expression.Call(stringMember, method, constant) : null;
        }

        private static object? ConvertFilterValue(object? value, Type targetType)
        {
            if (value == null) return null;

            try
            {
                if (value is JsonElement je)
                {
                    return je.ValueKind switch
                    {
                        JsonValueKind.Number when targetType == typeof(int) => je.GetInt32(),
                        JsonValueKind.Number when targetType == typeof(long) => je.GetInt64(),
                        JsonValueKind.Number when targetType == typeof(decimal) => je.GetDecimal(),
                        JsonValueKind.String when targetType == typeof(string) => je.GetString(),
                        JsonValueKind.True or JsonValueKind.False when targetType == typeof(bool) => je.GetBoolean(),
                        JsonValueKind.String when targetType == typeof(DateTime) => je.GetDateTime(),
                        JsonValueKind.String when targetType == typeof(int) || targetType == typeof(int?) => int.Parse(je.GetString()!),
                        JsonValueKind.String when targetType == typeof(long) || targetType == typeof(long?) => long.Parse(je.GetString()!),
                        JsonValueKind.String when targetType == typeof(decimal) || targetType == typeof(decimal?) => decimal.Parse(je.GetString()!),
                        _ => Convert.ChangeType(je.ToString(), targetType)
                    };
                }

                // Handle string to numeric conversions
                if (value is string strValue)
                {
                    if (targetType == typeof(long) || targetType == typeof(long?))
                    {
                        if (long.TryParse(strValue, out var longVal))
                            return longVal;
                    }
                    else if (targetType == typeof(int) || targetType == typeof(int?))
                    {
                        if (int.TryParse(strValue, out var intVal))
                            return intVal;
                    }
                    else if (targetType == typeof(decimal) || targetType == typeof(decimal?))
                    {
                        if (decimal.TryParse(strValue, out var decVal))
                            return decVal;
                    }
                    else if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
                    {
                        if (DateTime.TryParse(strValue, out var dateVal))
                            return dateVal;
                    }
                    else if (targetType == typeof(bool) || targetType == typeof(bool?))
                    {
                        if (bool.TryParse(strValue, out var boolVal))
                            return boolVal;
                    }
                }

                // If it's already a primitive / string etc.
                return Convert.ChangeType(value, targetType);
            }
            catch
            {
                return null;
            }
        }

        private static string NormalizeOperation(string? operation)
        {
            if (string.IsNullOrWhiteSpace(operation))
                return "eq";

            var normalized = operation.ToLowerInvariant().Trim();
            
            // Map common aliases to standard operations
            return normalized switch
            {
                "equal" or "equals" or "==" => "eq",
                "notequal" or "notequals" or "!=" or "<>" => "ne",
                "greaterthan" or ">" => "gt",
                "lessthan" or "<" => "lt",
                "greaterthanorequal" or ">=" => "gte",
                "lessthanorequal" or "<=" => "lte",
                _ => normalized
            };
        }
    }
}
