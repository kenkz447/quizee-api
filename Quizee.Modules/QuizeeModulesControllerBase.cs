using DYNAMIC_LAMBDA_EXPRESSION;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quizee.Modules.ViewModels;
using Quizee.Modules.QueryModels;

namespace Quizee.Modules
{
    public class QuizeeModulesControllerBase : ControllerBase
    {
        public ActionResult<ResultList<T>> OkPagination<T>(IQueryable<T> source, UrlQueryModel query) where T : class, new()
        {
            if (source == null)
            {
                return new ResultList<T>();
            }

            var sourceFiltered = source;
            var lambda = query.GetLambda<T>(this.Request.Query);
            if (lambda != null)
            {
                sourceFiltered = sourceFiltered.Where(lambda);
            }

            var sorts = query.GetSortModels();
            if (sorts.Count > 0)
            {
                sourceFiltered = sourceFiltered.Sort(sorts);
            }

            var result = new ResultList<T>();

            result.Data = sourceFiltered.Skip(query.Start).Take(query.Count).ToList();

            var totalItem = source.Count();
            result.Pagination = new Pagination
            {
                TotalItem = totalItem,
                Page = query.Page,
                PageSize = query.PageSize
            };

            return result;
        }

        public ActionResult<ResultList<T>> OkPagination<T>(IEnumerable<T> source, UrlQueryModel query) where T : class, new()
        {
            return OkPagination(source.AsQueryable(), query);
        }
    }
}
