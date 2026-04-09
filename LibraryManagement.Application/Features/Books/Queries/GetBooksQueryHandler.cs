using MediatR;
using LibraryManagement.Application.Interfaces;

namespace LibraryManagement.Application.Features.Books.Queries;

public class GetBooksQueryHandler : IRequestHandler<GetBooksQuery, List<BookDto>>
{
    private readonly ICuonSachRepository _cuonSachRepository;

    public GetBooksQueryHandler(ICuonSachRepository cuonSachRepository)
    {
        _cuonSachRepository = cuonSachRepository;
    }

    public async Task<List<BookDto>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
    {
        var tatCaSach = await _cuonSachRepository.GetAllAsync();
        
        return tatCaSach.Select(s => new BookDto
        {
            MaVachRFID = s.MaVachRFID,
            TenSach = s.DauSach?.TenSach ?? "Không Tên",
            ISBN = s.ISBN,
            TrangThai = s.TrangThai.ToString()
        }).ToList();
    }
}
