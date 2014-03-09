using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DIMS.Core.Extensions
{
    public static class ExpressionExtensions
    {
        public static PropertyInfo GetProperty<T, TProperty>(this Expression<Func<T, TProperty>> expression)
        {
            var body = (MemberExpression)GetExpressionBody(expression);

            var property = GetProperty(body);

            return property;
        }

        public static string GetPropertyString<T, TProperty>(this Expression<Func<T, TProperty>> expression)
        {
            var body = GetExpressionBody(expression);

            var properties = Enumerable.Empty<PropertyInfo>();

            while (body.NodeType == ExpressionType.MemberAccess)
            {
                var member = (MemberExpression)body;

                var property = GetProperty(member);

                properties = property.Concat(properties);

                body = member.Expression;
            }

            return string.Join(".", properties.Select(x => x.Name));
        }

        public static MethodInfo GetMethod<T, TReturnType>(this Expression<Func<T, TReturnType>> expression)
        {
            var body = (MethodCallExpression)GetExpressionBody(expression);

            return body.Method;
        }

        private static PropertyInfo GetProperty(MemberExpression expression)
        {
            return (PropertyInfo)expression.Member;
        }

        private static Expression GetExpressionBody<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var body = expression.Body;

            if (expression.Body.NodeType == ExpressionType.Convert)
                body = ((UnaryExpression)expression.Body).Operand;

            return body;
        }

        public static Expression<Func<TReturnType>> GetLambda<TReturnType>(this MethodInfo methodInfo)
        {
            var returnType = typeof(TReturnType);

            Expression expression = Expression.Call(null, methodInfo);

            if (methodInfo.ReturnType != returnType)
                expression = Expression.Convert(expression, returnType);

            var lambda = Expression.Lambda<Func<TReturnType>>(expression);

            return lambda;
        }

        public static Expression<Func<T, TReturnType>> GetLambda<T, TReturnType>(this MethodInfo methodInfo)
        {
            var type = typeof(T);
            var returnType = typeof(TReturnType);

            var parameter = Expression.Parameter(type);

            Expression expression = parameter;

            if (methodInfo.GetParameters()[0].ParameterType != type)
                expression = Expression.Convert(expression, methodInfo.DeclaringType);

            expression = Expression.Call(null, methodInfo, expression);

            if (methodInfo.ReturnType != returnType)
                expression = Expression.Convert(expression, returnType);

            var lambda = Expression.Lambda<Func<T, TReturnType>>(expression, parameter);

            return lambda;
        }

        public static Expression<Func<T, TReturnType>> GetLambda<T, TReturnType>(this PropertyInfo propertyInfo)
        {
            var type = typeof(T);
            var returnType = typeof(TReturnType);

            var parameter = Expression.Parameter(type);

            Expression expression = parameter;

            if (propertyInfo.DeclaringType != type)
                expression = Expression.Convert(expression, propertyInfo.DeclaringType);

            expression = Expression.Property(expression, propertyInfo);

            if (propertyInfo.PropertyType != returnType)
                expression = Expression.Convert(expression, returnType);

            var lambda = Expression.Lambda<Func<T, TReturnType>>(expression, parameter);

            return lambda;
        }
    }
}