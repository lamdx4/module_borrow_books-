using MediatR;

namespace LibraryManagement.Application.Features.Borrowing.Commands;

public class BorrowBookCommand : IRequest<bool>
{
    public string MaTheDocGia { get; set; } = string.Empty;
    public List<string> DanhSachMaVachRFID { get; set; } = new();
}