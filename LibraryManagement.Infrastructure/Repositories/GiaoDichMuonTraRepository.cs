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
        
        // Seed Data
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
