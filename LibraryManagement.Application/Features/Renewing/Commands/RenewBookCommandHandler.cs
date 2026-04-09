using MediatR;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Exceptions;

namespace LibraryManagement.Application.Features.Renewing.Commands;

public class RenewBookCommandHandler : IRequestHandler<RenewBookCommand, bool>
{
    private readonly IGiaoDichMuonTraRepository _giaoDichRepository;
    private readonly IPhieuDatTruocRepository _phieuDatTruocRepository;
    private readonly ICuonSachRepository _cuonSachRepository;
    private readonly IDocGiaRepository _docGiaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RenewBookCommandHandler(
        IGiaoDichMuonTraRepository giaoDichRepository,
        IPhieuDatTruocRepository phieuDatTruocRepository,
        ICuonSachRepository cuonSachRepository,
        IDocGiaRepository docGiaRepository,
        IUnitOfWork unitOfWork)
    {
        _giaoDichRepository = giaoDichRepository;
        _phieuDatTruocRepository = phieuDatTruocRepository;
        _cuonSachRepository = cuonSachRepository;
        _docGiaRepository = docGiaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RenewBookCommand request, CancellationToken cancellationToken)
    {
        var docGia = await _docGiaRepository.GetByMaTheAsync(request.MaTheDocGia);
        if (docGia == null) throw new Exception("Không tìm thấy thông tin độc giả.");

        foreach (var rfid in request.DanhSachMaVachRFID)
        {
            var trans = await _giaoDichRepository.GetActiveTransactionByBookAsync(rfid);
            if (trans == null || trans.MaThe != request.MaTheDocGia)
                continue;

            var cuonSach = await _cuonSachRepository.GetByMaVachAsync(rfid);
            if (cuonSach == null) continue;

            // 1. Check Reservation
            var reservation = await _phieuDatTruocRepository.GetActiveReservationByIsbnAsync(cuonSach.ISBN);
            if (reservation != null)
            {
                throw new BookAlreadyReservedException($"Không thể gia hạn sách {cuonSach.ISBN} vì đã có người đặt trước.");
            }

            // 2. Check Renew Limit
            if (trans.SoLanGiaHan >= 3) // Example limit
            {
                throw new LimitExceededException($"Sách {rfid} đã vượt quá số lần gia hạn tối đa.");
            }

            // 3. Check Overdue
            if (DateTime.Now > trans.NgayDenHan)
            {
                throw new BookOverdueException($"Sách {rfid} đã quá hạn trả, không thể gia hạn.");
            }

            // 4. Update
            trans.NgayDenHan = trans.NgayDenHan.AddDays(7);
            trans.SoLanGiaHan++;
            await _giaoDichRepository.UpdateAsync(trans);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
