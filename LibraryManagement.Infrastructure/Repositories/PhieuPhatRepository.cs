using System.Collections.Concurrent;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Infrastructure.Repositories;

public class PhieuPhatRepository : IPhieuPhatRepository
{
    private readonly ConcurrentDictionary<string, PhieuPhat> _data;

    public PhieuPhatRepository()
    {
        _data = new ConcurrentDictionary<string, PhieuPhat>();
        
        // Seed Data
        var phieu1 = new PhieuPhat
        {
            Id = "PP001",
            MaThe = "THE001",
            LyDoPhat = "Tra tre han",
            SoTienPhat = 50000,
            NgayLapPhieu = DateTime.Now.AddDays(-2),
            TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan
        };
        _data.TryAdd(phieu1.Id, phieu1);
    }

    public Task<IEnumerable<PhieuPhat>> GetUnpaidFinesByReaderAsync(string maThe)
    {
        var phieuPhats = _data.Values.Where(x => x.MaThe == maThe && x.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan).ToList();
        return Task.FromResult<IEnumerable<PhieuPhat>>(phieuPhats);
    }

    public Task AddAsync(PhieuPhat phieuPhat)
    {
        _data.TryAdd(phieuPhat.Id, phieuPhat);
        return Task.CompletedTask;
    }

    public Task UpdateManyAsync(IEnumerable<PhieuPhat> phieuPhats)
    {
        foreach (var phieuPhat in phieuPhats)
        {
            _data.AddOrUpdate(phieuPhat.Id, phieuPhat, (key, oldValue) => phieuPhat);
        }
        return Task.CompletedTask;
    }
}
