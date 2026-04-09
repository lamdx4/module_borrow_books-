using MediatR;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Exceptions;

namespace LibraryManagement.Application.Features.Reserving.Commands;

public class ReserveBookCommandHandler : IRequestHandler<ReserveBookCommand, bool>
{
    private readonly IDocGiaRepository _docGiaRepository;
    private readonly ICuonSachRepository _cuonSachRepository;
    private readonly IPhieuDatTruocRepository _phieuDatTruocRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReserveBookCommandHandler(
        IDocGiaRepository docGiaRepository,
        ICuonSachRepository cuonSachRepository,
        IPhieuDatTruocRepository phieuDatTruocRepository,
        IUnitOfWork unitOfWork)
    {
        _docGiaRepository = docGiaRepository;
        _cuonSachRepository = cuonSachRepository;
        _phieuDatTruocRepository = phieuDatTruocRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ReserveBookCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate Reader
        var docGia = await _docGiaRepository.GetByMaTheAsync(request.MaTheDocGia);
        if (docGia == null) throw new Exception("Không tìm thấy thông tin độc giả.");
        if (docGia.TrangThaiTaiKhoan == TrangThaiTaiKhoan.Khoa) throw new ReaderLockedException();
        if (docGia.NgayHetHanThe < DateTime.Now) throw new ReaderExpiredException();

        // 2. Check if copies are available
        var books = await _cuonSachRepository.GetByIsbnAsync(request.DauSachId);
        if (books != null && books.Any(b => b.TrangThai == TrangThaiCuonSach.SanSang))
        {
            throw new CopiesAvailableException();
        }

        // 3. Check existing reservation by this reader
        var existingReservations = await _phieuDatTruocRepository.GetActiveReservationsByReaderAsync(request.MaTheDocGia);
        if (existingReservations.Any(r => r.ISBN == request.DauSachId))
        {
            throw new AlreadyReservedException();
        }
        
        if (existingReservations.Count() >= 3)
        {
             throw new LimitExceededException("Bạn đã đạt giới hạn đặt trước tối đa (3 cuốn).");
        }

        // 4. Create Reservation
        var phieu = new PhieuDatTruoc
        {
            Id = Guid.NewGuid().ToString(),
            MaThe = request.MaTheDocGia,
            ISBN = request.DauSachId,
            NgayDat = DateTime.Now,
            NgayHetHanNhan = DateTime.Now.AddDays(30),
            TrangThai = "Đang chờ"
        };

        await _phieuDatTruocRepository.AddAsync(phieu);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
