using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Interfaces;

public interface IDocGiaRepository
{
    Task<DocGia?> GetByMaTheAsync(string maThe);
    Task UpdateAsync(DocGia docGia);
}
