using System;
using System.Collections.Generic;
using System.Text;

namespace DYNAMIC_LAMBDA_EXPRESSION
{
    public class ExpressionSortModel
    {
        public string Column { get; set; }
        public string Sort { get; set; }

        public string PairAsSqlExpression
        {
            get
            {
                return $"{Column} {Sort}";
            }
        }
    }
}
