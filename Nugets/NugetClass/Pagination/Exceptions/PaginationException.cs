using System.ComponentModel.DataAnnotations;

namespace NugetClass.Pagination.Exceptions;

public sealed class PaginationException : ValidationException
{
    public PaginationException(string message) : base(message) { }
    public PaginationException(string message, Exception innerException) : base(message, innerException) { }
}
