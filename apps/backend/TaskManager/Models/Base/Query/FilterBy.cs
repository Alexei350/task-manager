using System;
using System.Linq.Expressions;
using Newtonsoft.Json;
using TaskManager.Models.Base.Dynamic;
using TaskManager.Models.Base.Entities;
using TaskManager.Utils.Extensions;

namespace TaskManager.Models.Base.Query
{
    public class FilterBy<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>> Filter { get; private set; }

        public FilterBy(Expression<Func<T, bool>> filter)
        {
            Filter = filter;
        }

        public FilterBy(string filter)
        {
            if (filter.IsNullOrEmpty())
                return;

            try 
            {
                var dynamicFilter = JsonConvert.DeserializeObject<DynamicFilter>(filter);
                
                Filter = FromDynamicFilter(dynamicFilter);
            }
            catch
            {
                // ignored
            }
        }

        public FilterBy(DynamicFilter dynamicFilter)
        {
            Filter = FromDynamicFilter(dynamicFilter);
        }

        /// <summary>
        /// Adiciona uma expressão Lambda com o conector AND
        /// </summary>
        /// <param name="expression">Expressão a ser adicionada</param>
        /// <returns></returns>
        public FilterBy<T> AddExpression(Expression<Func<T, bool>> expression)
        {
            if (Filter == null)
            {
                Filter = expression;
            }
            else
            {
                var parameter = Expression.Parameter(typeof(T));

                var combinedBody = Expression.AndAlso(
                    Expression.Invoke(Filter, parameter),
                    Expression.Invoke(expression, parameter));

                Filter = Expression.Lambda<Func<T, bool>>(combinedBody, parameter);
            }

            return this;
        }
        
        /// <summary>
        /// Adiciona uma expressão Lambda com o conector OR
        /// </summary>
        /// <param name="expression">Expressão a ser adicionada</param>
        /// <returns></returns>
        public FilterBy<T> AddOrExpression(Expression<Func<T, bool>> expression)
        {
            if (Filter == null)
            {
                Filter = expression;
            }
            else
            {
                var parameter = Expression.Parameter(typeof(T));

                var combinedBody = Expression.OrElse(
                    Expression.Invoke(Filter, parameter),
                    Expression.Invoke(expression, parameter));

                Filter = Expression.Lambda<Func<T, bool>>(combinedBody, parameter);
            }

            return this;
        }

        /// <summary>
        /// Cria um instância do filtro mediante um filtro dinâmico
        /// </summary>
        /// <param name="dynamicFilter">Filtro dinâmico</param>
        /// <returns></returns>
        private static Expression<Func<T, bool>> FromDynamicFilter(DynamicFilter dynamicFilter)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression expression = null;

            if (dynamicFilter == null)
                return null;

            foreach (var filterItem in dynamicFilter.Values)
            {
                Expression propertyExpression = Expression.Property(parameter, filterItem.PropertyName);
                Expression constantExpression = Expression.Constant(filterItem.Value);

                Expression comparisonExpression = filterItem.Operation switch
                {
                    "Contains" => Expression.Call(propertyExpression,
                        typeof(string).GetMethod("Contains", [typeof(string)])!,
                        Expression.Convert(constantExpression, typeof(string))),
                    "Equals" => Expression.Equal(propertyExpression, constantExpression),
                    "GreaterThanOrEquals" => Expression.GreaterThanOrEqual(propertyExpression, constantExpression),
                    "LessThanOrEquals" => Expression.LessThanOrEqual(propertyExpression, constantExpression),
                    _ => null
                };

                if (expression == null)
                {
                    expression = comparisonExpression;
                }
                else if (comparisonExpression != null)
                {
                    expression = filterItem.Operation.ToLower() switch
                    {
                        "and" => Expression.AndAlso(expression, comparisonExpression),
                        "or" => Expression.OrElse(expression, comparisonExpression),
                        _ => expression
                    };
                }
            }

            return Expression.Lambda<Func<T, bool>>(expression!, parameter);
        }
    }
}