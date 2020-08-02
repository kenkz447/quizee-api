using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DYNAMIC_LAMBDA_EXPRESSION
{
    public class ExpressionCriteria<T>
    {
        private string _andOr = "And";
        private readonly List<ExpressionCriterion<T>> _expressionCriterions = new List<ExpressionCriterion<T>>();

        public ExpressionCriteria<T> And()
        {
            _andOr = "And";
            return this;
        }

        public ExpressionCriteria<T> Or()
        {
            _andOr = "Or";
            return this;
        }

        public ExpressionCriteria<T> Add(string propertyName, object value, ExpressionType expressionType)
        {
            var newCriterion = new ExpressionCriterion<T>(propertyName, value, expressionType, _andOr);
            _expressionCriterions.Add(newCriterion);
            return this;
        }
        public ExpressionCriteria<T> Add(string propertyName, object value, CustomExpressionType customExpressionType)
        {
            var newCriterion = new ExpressionCriterion<T>(propertyName, value, customExpressionType, _andOr);
            _expressionCriterions.Add(newCriterion);
            return this;
        }

        public Expression<Func<T, bool>> GetLambda()
        {
            Expression expression = null;
            
            var parameterExpression = Expression.Parameter(typeof(T), typeof(T).Name.ToLower());
            foreach (var item in _expressionCriterions)
            {
                if (expression == null)
                {
                    expression = GetExpression(parameterExpression, item);
                }
                else
                {
                    expression = item.AndOr == "And"
                        ? Expression.And(expression, GetExpression(parameterExpression, item))
                        : Expression.Or(expression, GetExpression(parameterExpression, item));
                }
            }

            return expression != null
                ? Expression.Lambda<Func<T, bool>>(expression, parameterExpression)
                : null;
        }

        private Expression GetExpression(ParameterExpression parameter, ExpressionCriterion<T> expressionCriteria)
        {
            Expression expression = parameter;
            foreach (var member in expressionCriteria.PropertyName.Split('.'))
            {
                expression = Expression.PropertyOrField(expression, member);
            }

            if (expressionCriteria.ExpressionTypeCustom.HasValue)
            {
                return expressionCriteria.GetExpressionCustom(expression);
            }

            return Expression.MakeBinary(expressionCriteria.ExpressionType, expression, expressionCriteria.Constant);
        }
    }
}
