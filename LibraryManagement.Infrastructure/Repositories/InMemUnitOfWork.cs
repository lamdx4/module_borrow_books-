using LibraryManagement.Application.Interfaces;

namespace LibraryManagement.Infrastructure.Repositories;

public class InMemUnitOfWork : IUnitOfWork
{
    public InMemUnitOfWork()
    {
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Vì sử dụng ConcurrentDictionary (In-Memory) nên các thay đổi đã được cập nhật trực tiếp qua AddOrUpdate.
        // Hàm này mô phỏng để tuân thủ UnitOfWork Pattern.
        return Task.FromResult(1);
    }
}
