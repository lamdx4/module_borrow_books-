using MediatR;

namespace LibraryManagement.Application.Features.Reserving.Commands;

public class ReserveBookCommand : IRequest<bool>
{
    public string MaTheDocGia { get; set; } = string.Empty;
    public string DauSachId { get; set; } = string.Empty; // using ISBN mapped as DauSachId logically in user requirement
}
