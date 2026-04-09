using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Interfaces;

public interface IGiaoDichMuonTraRepository
{
    Task<GiaoDichMuonTra?> GetActiveTransactionByBookAsync(string maVachRfid);
    Task<IEnumerable<GiaoDichMuonTra>> GetActiveTransactionsByReaderAsync(string maThe);
    Task<IEnumerable<GiaoDichMuonTra>> GetAllActiveTransactionsAsync();
    Task AddAsync(GiaoDichMuonTra giaoDich);
    Task UpdateAsync(GiaoDichMuonTra giaoDich);
}
