using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DYNAMIC_LAMBDA_EXPRESSION
{
    public static class ExpressionTypes
    {
        private static readonly string _sym = "_";

        public static ExpressionType EQ = ExpressionType.Equal;
        public static ExpressionType NE = ExpressionType.NotEqual;

        public static ExpressionType GT = ExpressionType.GreaterThan;
        public static ExpressionType GTE = ExpressionType.GreaterThanOrEqual;

        public static ExpressionType LT = ExpressionType.LessThan;
        public static ExpressionType LTE = ExpressionType.LessThanOrEqual;

        public static CustomExpressionType CONTAINS = CustomExpressionType.Contains;
        public static CustomExpressionType CONTAINSS = CustomExpressionType.ContainsCaseSentive;
        public static CustomExpressionType NULL = CustomExpressionType.Null;

        public static string GetProperyName(string raw)
        {
            return raw.Split(_sym)[0];
        }

        public static CustomExpressionType? GetCustomExpressionType(string propertyPath)
        {
            var propertyPathParts = propertyPath.Split(_sym);
            if (propertyPathParts.Length == 1)
            {
                return null;
            }

            var expressionStr = propertyPathParts[1];

            if (expressionStr == "contains")
            {
                return CONTAINS;
            }

            if (expressionStr == "containss")
            {
                return CONTAINSS;
            }

            if (expressionStr == "null")
            {
                return NULL;
            }

            return null;
        }

        public static ExpressionType? GetExpressionType(string propertyPath)
        {
            var propertyPathParts = propertyPath.Split(_sym);
            if (propertyPathParts.Length == 1)
            {
                return EQ;
            }

            var expressionStr = propertyPathParts[1];
            if (expressionStr == "eq")
            {
                return EQ;
            }

            if (expressionStr == "ne")
            {
                return NE;
            }

            if (expressionStr == "gt")
            {
                return GT;
            }

            if (expressionStr == "gte")
            {
                return GTE;
            }

            if (expressionStr == "lt")
            {
                return LT;
            }

            if (expressionStr == "lte")
            {
                return LTE;
            }

            return null;
        }
    }
}
