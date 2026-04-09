using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Interfaces;

public interface IPhieuDatTruocRepository
{
    Task<PhieuDatTruoc?> GetActiveReservationByIsbnAsync(string isbn);
    Task<IEnumerable<PhieuDatTruoc>> GetActiveReservationsByReaderAsync(string maThe);
    Task AddAsync(PhieuDatTruoc phieuDatTruoc);
    Task UpdateAsync(PhieuDatTruoc phieuDatTruoc);
}
