using NugetClass.Pagination.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;


namespace NugetClass.Pagination.Filters
{
    public static class Filter
    {
        public static Expression<Func<TModel, bool>> FromStringExpression<TModel>(
            string query,
            string parameter = "x")
        {
            try
            {
                ParameterExpression parameterExpression =
                    Expression.Parameter(typeof(TModel), parameter);

                return (Expression<Func<TModel, bool>>)DynamicExpressionParser.ParseLambda(
                    new ParameterExpression[1] { parameterExpression },
                    null,
                    query);
            }
            catch (Exception ex)
            {
                // Fix #2: preservar la causa original para debugging
                throw new PaginationException($"filter expression invalid: '{query}'", ex);
            }
        }

        /// <summary>
        /// Opcional: convierte "CreatedAt desc, Id asc" en IOrderedQueryable.
        /// </summary>
        public static IOrderedQueryable<TModel> ApplySort<TModel>(
            IQueryable<TModel> query,
            string? sortBy,
            string defaultSortField = "Id")
        {
            var expression = string.IsNullOrWhiteSpace(sortBy) ? defaultSortField : sortBy;
            try
            {
                return query.OrderBy(expression);
            }
            catch (Exception ex)
            {
                throw new PaginationException($"sort expression invalid: '{expression}'", ex);
            }
        }
    }
}
