using MediatR;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.Features.Fines.Commands;

public class PayFineCommandHandler : IRequestHandler<PayFineCommand, double>
{
    private readonly IPhieuPhatRepository _phieuPhatRepository;
    private readonly IDocGiaRepository _docGiaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PayFineCommandHandler(
        IPhieuPhatRepository phieuPhatRepository,
        IDocGiaRepository docGiaRepository,
        IUnitOfWork unitOfWork)
    {
        _phieuPhatRepository = phieuPhatRepository;
        _docGiaRepository = docGiaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<double> Handle(PayFineCommand request, CancellationToken cancellationToken)
    {
        var unpaidFines = await _phieuPhatRepository.GetUnpaidFinesByReaderAsync(request.MaTheDocGia);
        if (!unpaidFines.Any())
            return 0;

        double totalPaid = 0;
        foreach (var fine in unpaidFines)
        {
            fine.TrangThaiThanhToan = TrangThaiThanhToan.DaThanhToan;
            totalPaid += fine.SoTienPhat;
        }

        await _phieuPhatRepository.UpdateManyAsync(unpaidFines);

        var docGia = await _docGiaRepository.GetByMaTheAsync(request.MaTheDocGia);
        if (docGia != null)
        {
            docGia.TrangThaiTaiKhoan = TrangThaiTaiKhoan.BinhThuong;
            await _docGiaRepository.UpdateAsync(docGia);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return totalPaid; // Trả về số tiền đã thu để in biên lai
    }
}
