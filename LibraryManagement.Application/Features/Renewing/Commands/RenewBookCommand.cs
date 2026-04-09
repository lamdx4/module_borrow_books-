using MediatR;

namespace LibraryManagement.Application.Features.Renewing.Commands;

public class RenewBookCommand : IRequest<bool>
{
    public string MaTheDocGia { get; set; } = string.Empty;
    public List<string> DanhSachMaVachRFID { get; set; } = new();
}
