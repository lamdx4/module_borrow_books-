using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Interfaces;

public interface ICuonSachRepository
{
    Task<CuonSach?> GetByMaVachAsync(string maVachRfid);
    Task<IEnumerable<CuonSach>> GetByIsbnAsync(string isbn);
    Task<IEnumerable<CuonSach>> GetAllAsync();
    Task UpdateAsync(CuonSach cuonSach);
}
