using NugetClass.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetClass.Pagination.Models
{
    public class RequestParameters
    {
        private int _pageSize = PaginationDefaults.DefaultPageSize;
        private int _pageNumber = PaginationDefaults.DefaultPageNumber;

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value switch
            {
                < 1 => 1,
                > PaginationDefaults.MaxPageSize => PaginationDefaults.MaxPageSize,
                _ => value
            };
        }

        /// <summary>Ej: Status == "Open" &amp;&amp; Total &gt; 100</summary>
        public string? Filter { get; set; }

        /// <summary>Ej: "CreatedAt desc, Id asc"</summary>
        public string? SortBy { get; set; }
    }
}
