using System.Collections.Concurrent;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Infrastructure.Repositories;

public class DocGiaRepository : IDocGiaRepository
{
    private readonly ConcurrentDictionary<string, DocGia> _data;

    public DocGiaRepository()
    {
        _data = new ConcurrentDictionary<string, DocGia>();
        
        // Seed Data
        var docGia1 = new DocGia
        {
            Id = "DG001",
            HoTen = "Nguyen Van A",
            Email = "nva@gmail.com",
            SoDienThoai = "0123456789",
            MaThe = "THE001",
            LoaiThe = "SinhVien",
            NgayHetHanThe = DateTime.Now.AddYears(1),
            SoSachDangMuon = 0,
            TongNoPhat = 0,
            TrangThaiTaiKhoan = TrangThaiTaiKhoan.BinhThuong
        };

        var docGia2 = new DocGia
        {
            Id = "DG002",
            HoTen = "Tran Thi B",
            Email = "ttb@gmail.com",
            SoDienThoai = "0987654321",
            MaThe = "THE002",
            LoaiThe = "GiangVien",
            NgayHetHanThe = DateTime.Now.AddDays(-1), // Expired
            SoSachDangMuon = 0,
            TongNoPhat = 0,
            TrangThaiTaiKhoan = TrangThaiTaiKhoan.BinhThuong
        };

        _data.TryAdd(docGia1.MaThe, docGia1);
        _data.TryAdd(docGia2.MaThe, docGia2);
    }

    public Task<DocGia?> GetByMaTheAsync(string maThe)
    {
        _data.TryGetValue(maThe, out var docGia);
        return Task.FromResult(docGia);
    }

    public Task UpdateAsync(DocGia docGia)
    {
        _data.AddOrUpdate(docGia.MaThe, docGia, (key, oldValue) => docGia);
        return Task.CompletedTask;
    }
}
