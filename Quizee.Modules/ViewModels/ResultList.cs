using System;
using System.Collections.Generic;
using System.Text;

namespace Quizee.Modules.ViewModels
{
    public class ResultList<T>
    {
        public List<T> Data { get; set; }
        public Pagination Pagination { get; set; }
    }

    public class Pagination
    {
        public int TotalItem { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
