using MediatR;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.Features.Books.Queries;

public class BookDto
{
    public string MaVachRFID { get; set; } = string.Empty;
    public string TenSach { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string TrangThai { get; set; } = string.Empty;
}

public class GetBooksQuery : IRequest<List<BookDto>>
{
}
