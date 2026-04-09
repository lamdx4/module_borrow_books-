using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Interfaces;

public interface IPhieuPhatRepository
{
    Task<IEnumerable<PhieuPhat>> GetUnpaidFinesByReaderAsync(string maThe);
    Task AddAsync(PhieuPhat phieuPhat);
    Task UpdateManyAsync(IEnumerable<PhieuPhat> phieuPhats);
}
