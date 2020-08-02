using DYNAMIC_LAMBDA_EXPRESSION;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Quizee.Modules.QueryModels
{
    public class UrlQueryModel
    {
        public int Count
        {
            get
            {
                return PageSize;
            }
        }

        public int Start
        {
            get
            {
                return (Page - 1) * PageSize;
            }
        }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public static readonly string FilterKey = "filter.";

        public Dictionary<string, object> Filter { get; set; }

        public string Sort { get; set; }

        public Expression<Func<T, bool>> GetLambda<T>(IQueryCollection query) where T : class, new()
        {
            var filters = query.Where((o) => o.Key.StartsWith(FilterKey, true, null) && o.Value.Count() > 0);
            if (filters.Count() == 0)
            {
                return null;
            }

            var expressionCriteria = new ExpressionCriteria<T>();
            foreach (var entry in filters)
            {
                var filterPath = entry.Key.Remove(0, FilterKey.Length);

                expressionCriteria = expressionCriteria.And();

                foreach (var filterValue in entry.Value)
                {
                    var propertyPath = ExpressionTypes.GetProperyName(filterPath);
                    var customexpressionType = ExpressionTypes.GetCustomExpressionType(filterPath);
                    var expressionType = ExpressionTypes.GetExpressionType(filterPath);

                    if (customexpressionType.HasValue)
                    {
                        expressionCriteria.Add(propertyPath, filterValue, customexpressionType.Value);
                    }
                    else if (expressionType.HasValue)
                    {
                        expressionCriteria.Add(propertyPath, filterValue, expressionType.Value);
                    }

                    expressionCriteria.Or();
                }
            }

            return expressionCriteria.GetLambda();
        }

        public List<ExpressionSortModel> GetSortModels()
        {
            var result = new List<ExpressionSortModel>();

            if (string.IsNullOrEmpty(Sort))
            {
                return result;
            }

            var sorts = Sort.Split(',');

            foreach (var sort in sorts)
            {
                var sortParts = sort.Split(':');
                var sortField = sortParts[0];
                var sortModel = sortParts[1];

                result.Add(new ExpressionSortModel
                {
                    Column = sortField,
                    Sort = sortModel
                });
            }

            return result;
        }
    }
}