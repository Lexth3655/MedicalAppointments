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
                throw new ValidationException($"filter expression invalid: '{query}'", ex);
            }
        }

        /// <summary>
        /// Opcional: convierte "CreatedAt desc, Id asc" en IOrderedQueryable.
        /// </summary>
        public static IOrderedQueryable<TModel> ApplySort<TModel>(
            IQueryable<TModel> query,
            string? sortBy)
        {
            try
            {
                return string.IsNullOrWhiteSpace(sortBy)
                    ? query.OrderBy("Id")           // Fix #3: orden por defecto determinista
                    : query.OrderBy(sortBy);
            }
            catch (Exception ex)
            {
                throw new ValidationException($"sort expression invalid: '{sortBy}'", ex);
            }
        }
    }
}
