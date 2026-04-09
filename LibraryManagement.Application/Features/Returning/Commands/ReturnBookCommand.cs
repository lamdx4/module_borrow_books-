using MediatR;

namespace LibraryManagement.Application.Features.Returning.Commands;

public class ReturnBookCommand : IRequest<bool>
{
    public List<string> DanhSachMaVachRFID { get; set; } = new();
    public string TinhTrangKiemTra { get; set; } = "Bình thường";
}
