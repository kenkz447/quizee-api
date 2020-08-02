using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DYNAMIC_LAMBDA_EXPRESSION
{
    internal class ExpressionCriterion<T>
    {
        public ExpressionCriterion(
           string propertyName,
           object value,
           ExpressionType expressionType,
           string andOr = "And")
        {
            var propertyInfo = GetTypePropertyInfo(typeof(T), propertyName);
            Constant = ParseValue(propertyInfo, value);

            AndOr = andOr;
            PropertyName = propertyName;
            ExpressionType = expressionType;
        }

        public ExpressionCriterion(
           string propertyName,
           object value,
           CustomExpressionType customExpressionType,
           string andOr = "And")
        {
            var propertyInfo = GetTypePropertyInfo(typeof(T), propertyName);

            Constant = ParseValue(customExpressionType, propertyInfo, value);

            AndOr = andOr;
            PropertyName = propertyName;
            ExpressionTypeCustom = customExpressionType;
        }

        public Expression GetExpressionCustom(Expression member)
        {
            var nullCheckProp = Expression.Equal(member, Expression.Constant(null, member.Type));

            if (this.ExpressionTypeCustom == CustomExpressionType.Contains)
            {
                var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                var left = Expression.Call(member, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
                var right = Expression.Call(this.Constant, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));

                var originalExpression = Expression.Call(left, method, right);
                return Expression.Condition(nullCheckProp, Expression.Constant(false), originalExpression);
            }

            if (this.ExpressionTypeCustom == CustomExpressionType.ContainsCaseSentive)
            {
                var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                var originalExpression = Expression.Call(member, method, this.Constant);

                return Expression.Condition(nullCheckProp, Expression.Constant(false), originalExpression);
            }

            if (ExpressionTypeCustom == CustomExpressionType.Null)
            {
                if ((bool)Constant.Value == true)
                {
                    return nullCheckProp;
                }

                return Expression.Not(nullCheckProp);
            }

            return null;
        }

        private PropertyInfo GetTypePropertyInfo(Type type, string propertyName)
        {
            string[] parts = propertyName.Split('.');
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;

            var info = (parts.Length > 1)
                ? GetTypePropertyInfo(type.GetProperty(parts[0], flags).PropertyType, parts.Skip(1).Aggregate((a, i) => a + "." + i))
                : type.GetProperty(propertyName, flags);

            if (info == null)
            {
                throw new ArgumentException(propertyName, $"Property {propertyName}  is not a member of {type.Name}");
            }

            return info;
        }

        private ConstantExpression ParseValue(CustomExpressionType customExpressionType, PropertyInfo propertyInfo, object rawValue)
        {
            var value = rawValue.ToString();

            if (customExpressionType == CustomExpressionType.Null)
            {
                if (bool.TrueString.ToLower() == value.ToLower() && value == "true")
                {
                    return Expression.Constant(true);
                }

                return Expression.Constant(false);
            }

            return ParseValue(propertyInfo, rawValue);
        }

        private ConstantExpression ParseValue(PropertyInfo propertyInfo, object rawValue)
        {
            var propertyType = propertyInfo.PropertyType;
            var value = rawValue.ToString();

            if (value == "null")
            {
                return null;
            }

            if (propertyType == typeof(int) || propertyType == typeof(int?))
            {
                return Expression.Constant(int.Parse(value));
            }

            if (propertyType == typeof(long))
            {
                return Expression.Constant(long.Parse(value));
            }

            if (propertyType == typeof(long?))
            {
                return Expression.Constant(long.Parse(value), typeof(long?));
            }

            if (propertyType == typeof(float) || propertyType == typeof(float?))
            {
                return Expression.Constant(float.Parse(value));
            }

            if (propertyType == typeof(double) || propertyType == typeof(double?))
            {
                return Expression.Constant(double.Parse(value));
            }

            if (propertyType == typeof(bool) || propertyType == typeof(bool?))
            {
                if (bool.TrueString.ToLower() == value.ToLower() && value == "true")
                {
                    return Expression.Constant(true);
                }

                return Expression.Constant(false);
            }

            if (propertyType == typeof(DateTime?))
            {
                return Expression.Constant(DateTime.Parse(value), typeof(DateTime?));
            }

            if (propertyType == typeof(DateTime))
            {
                return Expression.Constant(DateTime.Parse(value));
            }

            return Expression.Constant(value);
        }

        public string PropertyName { get; }
        public ConstantExpression Constant { get; }
        public MethodInfo MethodInfo { get; }
        public ExpressionType ExpressionType { get; }
        public CustomExpressionType? ExpressionTypeCustom { get; }
        public string AndOr { get; }
    }
}
