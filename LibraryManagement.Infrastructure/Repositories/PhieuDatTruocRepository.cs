using System.Collections.Concurrent;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Infrastructure.Repositories;

public class PhieuDatTruocRepository : IPhieuDatTruocRepository
{
    private readonly ConcurrentDictionary<string, PhieuDatTruoc> _data;

    public PhieuDatTruocRepository()
    {
        _data = new ConcurrentDictionary<string, PhieuDatTruoc>();
        
        // Seed Data
        var phieu1 = new PhieuDatTruoc
        {
            Id = "PDT001",
            MaThe = "THE001",
            ISBN = "978-0131103627", // The C Programming Language
            NgayDat = DateTime.Now.AddDays(-1),
            TrangThai = "Đang chờ"
        };
        _data.TryAdd(phieu1.Id, phieu1);
    }

    public Task<PhieuDatTruoc?> GetActiveReservationByIsbnAsync(string isbn)
    {
        var phieu = _data.Values.FirstOrDefault(x => x.ISBN == isbn && x.TrangThai == "Đang chờ");
        return Task.FromResult(phieu);
    }

    public Task<IEnumerable<PhieuDatTruoc>> GetActiveReservationsByReaderAsync(string maThe)
    {
        var phieus = _data.Values.Where(x => x.MaThe == maThe && x.TrangThai == "Đang chờ").ToList();
        return Task.FromResult<IEnumerable<PhieuDatTruoc>>(phieus);
    }

    public Task AddAsync(PhieuDatTruoc phieuDatTruoc)
    {
        _data.TryAdd(phieuDatTruoc.Id, phieuDatTruoc);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(PhieuDatTruoc phieuDatTruoc)
    {
        _data.AddOrUpdate(phieuDatTruoc.Id, phieuDatTruoc, (key, oldValue) => phieuDatTruoc);
        return Task.CompletedTask;
    }
}
