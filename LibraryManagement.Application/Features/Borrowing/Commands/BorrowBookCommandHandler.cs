using MediatR;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Exceptions;

namespace LibraryManagement.Application.Features.Borrowing.Commands;

public class BorrowBookCommandHandler : IRequestHandler<BorrowBookCommand, bool>
{
    private readonly IDocGiaRepository _docGiaRepository;
    private readonly ICuonSachRepository _cuonSachRepository;
    private readonly IGiaoDichMuonTraRepository _giaoDichRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BorrowBookCommandHandler(
        IDocGiaRepository docGiaRepository,
        ICuonSachRepository cuonSachRepository,
        IGiaoDichMuonTraRepository giaoDichRepository,
        IUnitOfWork unitOfWork)
    {
        _docGiaRepository = docGiaRepository;
        _cuonSachRepository = cuonSachRepository;
        _giaoDichRepository = giaoDichRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(BorrowBookCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch reader
        var docGia = await _docGiaRepository.GetByMaTheAsync(request.MaTheDocGia);
        if (docGia == null) throw new Exception("Không tìm thấy thông tin Thẻ độc giả.");

        // 2 & 3. Validations
        if (docGia.NgayHetHanThe < DateTime.Now)
            throw new ReaderExpiredException();
        if (docGia.TrangThaiTaiKhoan == TrangThaiTaiKhoan.Khoa)
            throw new ReaderLockedException();
        
        // 4. Limit check
        if (docGia.SoSachDangMuon + request.DanhSachMaVachRFID.Count > 5)
            throw new LimitExceededException("Số sách mượn vượt quá hạn mức tối đa (5 cuốn).");

        // 5. Process each book
        var giaoDichMuonTras = new List<GiaoDichMuonTra>();
        foreach (var rfid in request.DanhSachMaVachRFID)
        {
            var cuonSach = await _cuonSachRepository.GetByMaVachAsync(rfid);
            if (cuonSach == null || cuonSach.TrangThai != TrangThaiCuonSach.SanSang)
            {
                throw new BookNotAvailableException($"Cuốn sách mã {rfid} không sẵn sàng để mượn.");
            }

            // Update state
            cuonSach.TrangThai = TrangThaiCuonSach.DangMuon;
            await _cuonSachRepository.UpdateAsync(cuonSach);

            // Create transaction
            var giaoDich = new GiaoDichMuonTra
            {
                Id = Guid.NewGuid().ToString(),
                MaThe = docGia.MaThe,
                MaVachRFID = rfid,
                NgayMuon = DateTime.Now,
                NgayDenHan = DateTime.Now.AddDays(14),
                SoLanGiaHan = 0,
                TrangThaiGD = TrangThaiGiaoDich.DangMuon
            };
            
            await _giaoDichRepository.AddAsync(giaoDich);
        }

        // 6. Update reader's book count
        docGia.CapNhatSoSachMuon(request.DanhSachMaVachRFID.Count);
        await _docGiaRepository.UpdateAsync(docGia);

        // 7. Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
