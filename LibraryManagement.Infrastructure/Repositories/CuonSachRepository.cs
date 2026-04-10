using System.Collections.Concurrent;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Infrastructure.Repositories;

public class CuonSachRepository : ICuonSachRepository
{
    private readonly ConcurrentDictionary<string, CuonSach> _data;
    private readonly ConcurrentDictionary<string, DauSach> _dauSachData;

    public CuonSachRepository()
    {
        _data = new ConcurrentDictionary<string, CuonSach>();
        _dauSachData = new ConcurrentDictionary<string, DauSach>();

        // Seed Data
        var dauSach1 = new DauSach { Id = "DS001", ISBN = "978-0131103627", TenSach = "The C Programming Language", TacGia = "Brian W. Kernighan", TheLoai = "IT", NhaXuatBan = "Prentice Hall" };
        var dauSach2 = new DauSach { Id = "DS002", ISBN = "978-0201633610", TenSach = "Design Patterns", TacGia = "Erich Gamma", TheLoai = "IT", NhaXuatBan = "Addison-Wesley" };
        var dauSach3 = new DauSach { Id = "DS003", ISBN = "978-0134494166", TenSach = "Clean Architecture", TacGia = "Robert C. Martin", TheLoai = "IT", NhaXuatBan = "Prentice Hall" };
        var dauSach4 = new DauSach { Id = "DS004", ISBN = "978-0321125217", TenSach = "Domain-Driven Design", TacGia = "Eric Evans", TheLoai = "IT", NhaXuatBan = "Addison-Wesley" };
        var dauSach5 = new DauSach { Id = "DS005", ISBN = "893-6052321584", TenSach = "Dế Mèn Phiêu Lưu Ký", TacGia = "Tô Hoài", TheLoai = "Văn Học", NhaXuatBan = "Kim Đồng" };

        _dauSachData.TryAdd(dauSach1.ISBN, dauSach1);
        _dauSachData.TryAdd(dauSach2.ISBN, dauSach2);
        _dauSachData.TryAdd(dauSach3.ISBN, dauSach3);
        _dauSachData.TryAdd(dauSach4.ISBN, dauSach4);
        _dauSachData.TryAdd(dauSach5.ISBN, dauSach5);

        var sach1 = new CuonSach { Id = "CS001", MaVachRFID = "RFID001", ISBN = dauSach1.ISBN, DauSach = dauSach1, TinhTrangVatLy = TinhTrangVatLy.BinhThuong, TrangThai = TrangThaiCuonSach.SanSang };
        var sach2 = new CuonSach { Id = "CS002", MaVachRFID = "RFID002", ISBN = dauSach1.ISBN, DauSach = dauSach1, TinhTrangVatLy = TinhTrangVatLy.BinhThuong, TrangThai = TrangThaiCuonSach.DangMuon };
        var sach3 = new CuonSach { Id = "CS003", MaVachRFID = "RFID003", ISBN = dauSach2.ISBN, DauSach = dauSach2, TinhTrangVatLy = TinhTrangVatLy.BinhThuong, TrangThai = TrangThaiCuonSach.DangMuon };
        var sach4 = new CuonSach { Id = "CS004", MaVachRFID = "RFID004", ISBN = dauSach3.ISBN, DauSach = dauSach3, TinhTrangVatLy = TinhTrangVatLy.BinhThuong, TrangThai = TrangThaiCuonSach.DangMuon };
        var sach5 = new CuonSach { Id = "CS005", MaVachRFID = "RFID005", ISBN = dauSach4.ISBN, DauSach = dauSach4, TinhTrangVatLy = TinhTrangVatLy.Uot, TrangThai = TrangThaiCuonSach.HuHong };
        var sach6 = new CuonSach { Id = "CS006", MaVachRFID = "RFID006", ISBN = dauSach5.ISBN, DauSach = dauSach5, TinhTrangVatLy = TinhTrangVatLy.BinhThuong, TrangThai = TrangThaiCuonSach.SanSang };
        var sach7 = new CuonSach { Id = "CS007", MaVachRFID = "RFID007", ISBN = dauSach5.ISBN, DauSach = dauSach5, TinhTrangVatLy = TinhTrangVatLy.BinhThuong, TrangThai = TrangThaiCuonSach.DaDatTruoc };
        var sach8 = new CuonSach { Id = "CS008", MaVachRFID = "RFID008", ISBN = dauSach5.ISBN, DauSach = dauSach5, TinhTrangVatLy = TinhTrangVatLy.BinhThuong, TrangThai = TrangThaiCuonSach.SanSang };

        _data.TryAdd(sach1.MaVachRFID, sach1);
        _data.TryAdd(sach2.MaVachRFID, sach2);
        _data.TryAdd(sach3.MaVachRFID, sach3);
        _data.TryAdd(sach4.MaVachRFID, sach4);
        _data.TryAdd(sach5.MaVachRFID, sach5);
        _data.TryAdd(sach6.MaVachRFID, sach6);
        _data.TryAdd(sach7.MaVachRFID, sach7);
        _data.TryAdd(sach8.MaVachRFID, sach8);
    }

    public Task<CuonSach?> GetByMaVachAsync(string maVachRfid)
    {
        _data.TryGetValue(maVachRfid, out var sach);
        return Task.FromResult(sach);
    }

    public Task<IEnumerable<CuonSach>> GetByIsbnAsync(string isbn)
    {
        var sachs = _data.Values.Where(x => x.ISBN == isbn).ToList();
        return Task.FromResult<IEnumerable<CuonSach>>(sachs);
    }

    public Task<IEnumerable<CuonSach>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<CuonSach>>(_data.Values.ToList());
    }

    public Task UpdateAsync(CuonSach cuonSach)
    {
        _data.AddOrUpdate(cuonSach.MaVachRFID, cuonSach, (key, oldValue) => cuonSach);
        return Task.CompletedTask;
    }
}
