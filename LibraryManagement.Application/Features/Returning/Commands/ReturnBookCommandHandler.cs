using MediatR;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.Features.Returning.Commands;

public class ReturnBookCommandHandler : IRequestHandler<ReturnBookCommand, bool>
{
    private readonly ICuonSachRepository _cuonSachRepository;
    private readonly IGiaoDichMuonTraRepository _giaoDichRepository;
    private readonly IPhieuPhatRepository _phieuPhatRepository;
    private readonly IDocGiaRepository _docGiaRepository;
    private readonly IPhieuDatTruocRepository _phieuDatTruocRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public ReturnBookCommandHandler(
        ICuonSachRepository cuonSachRepository,
        IGiaoDichMuonTraRepository giaoDichRepository,
        IPhieuPhatRepository phieuPhatRepository,
        IDocGiaRepository docGiaRepository,
        IPhieuDatTruocRepository phieuDatTruocRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _cuonSachRepository = cuonSachRepository;
        _giaoDichRepository = giaoDichRepository;
        _phieuPhatRepository = phieuPhatRepository;
        _docGiaRepository = docGiaRepository;
        _phieuDatTruocRepository = phieuDatTruocRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<bool> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
    {
        foreach (var rfid in request.DanhSachMaVachRFID)
        {
            var trans = await _giaoDichRepository.GetActiveTransactionByBookAsync(rfid);
            if (trans == null) continue; // Khong tim thay giao dich mượn cho sách này

            var cuonSach = await _cuonSachRepository.GetByMaVachAsync(rfid);
            if (cuonSach == null) continue;

            var docGia = await _docGiaRepository.GetByMaTheAsync(trans.MaThe);
            if (docGia == null) continue;

            bool isFined = false;

            // Xử lý phạt: Hư hỏng
            if (request.TinhTrangKiemTra == "Hư hỏng")
            {
                cuonSach.TrangThai = TrangThaiCuonSach.HuHong;
                var phieuPhat = new PhieuPhat
                {
                    Id = Guid.NewGuid().ToString(),
                    MaThe = trans.MaThe,
                    LyDoPhat = "Làm hỏng sách",
                    SoTienPhat = 100000,
                    NgayLapPhieu = DateTime.Now,
                    TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan
                };
                await _phieuPhatRepository.AddAsync(phieuPhat);
                isFined = true;
            }

            // Xử lý phạt: Trễ hạn
            if (DateTime.Now > trans.NgayDenHan)
            {
                int delayDays = (DateTime.Now - trans.NgayDenHan).Days;
                if (delayDays > 0)
                {
                    var phieuPhat = new PhieuPhat
                    {
                        Id = Guid.NewGuid().ToString(),
                        MaThe = trans.MaThe,
                        LyDoPhat = $"Trả sách trễ hạn {delayDays} ngày",
                        SoTienPhat = delayDays * 5000,
                        NgayLapPhieu = DateTime.Now,
                        TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan
                    };
                    await _phieuPhatRepository.AddAsync(phieuPhat);
                    isFined = true;
                }
            }

            if (isFined)
            {
                docGia.TrangThaiTaiKhoan = TrangThaiTaiKhoan.Khoa;
            }

            if (request.TinhTrangKiemTra != "Hư hỏng")
            {
                // Xử lý đặt trước sách
                var reservation = await _phieuDatTruocRepository.GetActiveReservationByIsbnAsync(cuonSach.ISBN);
                if (reservation != null)
                {
                    cuonSach.TrangThai = TrangThaiCuonSach.DaDatTruoc;
                    // Bắn notification nếu cần: await _mediator.Publish(new BookReservedEvent(...));
                }
                else
                {
                    cuonSach.TrangThai = TrangThaiCuonSach.SanSang;
                }
            }

            // Cập nhật giao dịch và số sách
            trans.NgayTraThucTe = DateTime.Now;
            trans.TrangThaiGD = TrangThaiGiaoDich.DaTra;
            await _giaoDichRepository.UpdateAsync(trans);

            docGia.CapNhatSoSachMuon(-1);
            await _docGiaRepository.UpdateAsync(docGia);
            await _cuonSachRepository.UpdateAsync(cuonSach);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
