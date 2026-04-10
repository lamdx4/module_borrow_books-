using System.Collections.Concurrent;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Infrastructure.Repositories;

public class GiaoDichMuonTraRepository : IGiaoDichMuonTraRepository
{
    private readonly ConcurrentDictionary<string, GiaoDichMuonTra> _data;

    public GiaoDichMuonTraRepository()
    {
        _data = new ConcurrentDictionary<string, GiaoDichMuonTra>();
        
        var giaoDich1 = new GiaoDichMuonTra
        {
            Id = "GD001",
            MaThe = "THE001",
            MaVachRFID = "RFID002",
            NgayMuon = DateTime.Now.AddDays(-10),
            NgayDenHan = DateTime.Now.AddDays(4),
            SoLanGiaHan = 0,
            TrangThaiGD = TrangThaiGiaoDich.DangMuon
        };
        _data.TryAdd(giaoDich1.Id, giaoDich1);

        // Seed GD2: Quá hạn để test Exception "Đã quá hạn trả"
        var giaoDich2 = new GiaoDichMuonTra
        {
            Id = "GD002",
            MaThe = "THE001",
            MaVachRFID = "RFID003",
            NgayMuon = DateTime.Now.AddDays(-15),
            NgayDenHan = DateTime.Now.AddDays(-1), // Quá hạn 1 ngày
            SoLanGiaHan = 0,
            TrangThaiGD = TrangThaiGiaoDich.DangMuon
        };
        _data.TryAdd(giaoDich2.Id, giaoDich2);

        // Seed GD3: Hết lượt gia hạn
        var giaoDich3 = new GiaoDichMuonTra
        {
            Id = "GD003",
            MaThe = "THE001",
            MaVachRFID = "RFID004", // Phải đảm bảo RFID004 đang được đánh dấu là DangMuon trong CuonSach
            NgayMuon = DateTime.Now.AddDays(-20),
            NgayDenHan = DateTime.Now.AddDays(2),
            SoLanGiaHan = 3, // Vượt giới hạn
            TrangThaiGD = TrangThaiGiaoDich.DangMuon
        };
        _data.TryAdd(giaoDich3.Id, giaoDich3);
    }

    public Task<GiaoDichMuonTra?> GetActiveTransactionByBookAsync(string maVachRfid)
    {
        var giaoDich = _data.Values.FirstOrDefault(x => x.MaVachRFID == maVachRfid && x.TrangThaiGD == TrangThaiGiaoDich.DangMuon);
        return Task.FromResult(giaoDich);
    }

    public Task<IEnumerable<GiaoDichMuonTra>> GetActiveTransactionsByReaderAsync(string maThe)
    {
        var giaoDichs = _data.Values.Where(x => x.MaThe == maThe && x.TrangThaiGD == TrangThaiGiaoDich.DangMuon).ToList();
        return Task.FromResult<IEnumerable<GiaoDichMuonTra>>(giaoDichs);
    }

    public Task<IEnumerable<GiaoDichMuonTra>> GetAllActiveTransactionsAsync()
    {
        var giaoDichs = _data.Values.Where(x => x.TrangThaiGD == TrangThaiGiaoDich.DangMuon).ToList();
        return Task.FromResult<IEnumerable<GiaoDichMuonTra>>(giaoDichs);
    }

    public Task AddAsync(GiaoDichMuonTra giaoDich)
    {
        _data.TryAdd(giaoDich.Id, giaoDich);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(GiaoDichMuonTra giaoDich)
    {
        _data.AddOrUpdate(giaoDich.Id, giaoDich, (key, oldValue) => giaoDich);
        return Task.CompletedTask;
    }
}
