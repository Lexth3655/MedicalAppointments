namespace NugetClass.Pagination.Models
{
    public class PagedDto<T> where T : class
    {
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public T Data { get; set; } = default!;

        public PagedDto() { }

        public static PagedDto<T> From<TEntity>(PagedResult<TEntity> src, T data) => new()
        {
            TotalRecords = src.TotalRecords,
            TotalPages = src.TotalPages,
            CurrentPage = src.CurrentPage,
            PageSize = src.PageSize,
            Data = data
        };
    }
}
