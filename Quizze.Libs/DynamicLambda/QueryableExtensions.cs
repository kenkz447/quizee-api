using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace DYNAMIC_LAMBDA_EXPRESSION
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Sort<T>(this IQueryable<T> source, IEnumerable<ExpressionSortModel> sortModels)
        {
            var type = typeof(T);
            var expression = source.Expression;
            var count = 0;

            foreach (var item in sortModels)
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var propertyInfo = type.GetProperty(
                    item.Column,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase
                );

                if (propertyInfo == null)
                {
                    throw new ArgumentException(item.Column, $"Property {item.Column} is not a member of {type.Name}");
                }

                var selector = Expression.PropertyOrField(parameter, propertyInfo.Name);

                var method = string.Equals(item.Sort, "desc", StringComparison.OrdinalIgnoreCase) ?
                    (count == 0 ? "OrderByDescending" : "ThenByDescending") :
                    (count == 0 ? "OrderBy" : "ThenBy");
                expression = Expression.Call(typeof(Queryable), method,
                    new Type[] { source.ElementType, selector.Type },
                    expression, Expression.Quote(Expression.Lambda(selector, parameter)));

                count++;
            }

            return count > 0 ? source.Provider.CreateQuery<T>(expression) : source;
        }
    }
}
