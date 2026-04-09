using MediatR;

namespace LibraryManagement.Application.Features.Fines.Commands;

public class PayFineCommand : IRequest<double>
{
    public string MaTheDocGia { get; set; } = string.Empty;
}
