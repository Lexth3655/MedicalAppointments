using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetClass.Pagination.Exceptions;

public sealed class PaginationException : ValidationException
{
    public PaginationException(string message) : base(message) { }
    public PaginationException(string message, Exception innerException) : base(message, innerException) { }
}
