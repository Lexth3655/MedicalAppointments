using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetClass.Pagination.Models
{
    public class PagedResult<T>
    {
        public IReadOnlyList<T> Data { get; init; } = Array.Empty<T>();
        public int TotalRecords { get; init; }
        public int CurrentPage { get; init; }
        public int PageSize { get; init; }

        //  fix: Math.Ceiling, no división entera
        public int TotalPages => PageSize <= 0
            ? 0
            : (int)Math.Ceiling(TotalRecords / (double)PageSize);

        public bool HasNext => CurrentPage < TotalPages;
        public bool HasPrevious => CurrentPage > 1;
    }
}
