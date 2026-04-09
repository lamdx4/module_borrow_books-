using Grpc.Core;
using MediatR;
using LibraryManagement.Presentation.Protos;
using LibraryManagement.Application.Features.Borrowing.Commands;
using LibraryManagement.Application.Features.Returning.Commands;
using LibraryManagement.Application.Features.Renewing.Commands;
using LibraryManagement.Application.Features.Reserving.Commands;
using LibraryManagement.Application.Features.Fines.Commands;

namespace LibraryManagement.Presentation.Service;

public class LibraryGrpcService : LibraryService.LibraryServiceBase
{
    private readonly IMediator _mediator;

    public LibraryGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<BorrowResponse> BorrowBook(BorrowRequest request, ServerCallContext context)
    {
        try
        {
            var command = new BorrowBookCommand
            {
                MaTheDocGia = request.MaTheDocGia,
                DanhSachMaVachRFID = request.DanhSachMaVachRFID.ToList()
            };
            var result = await _mediator.Send(command);
            return new BorrowResponse { Success = result, Message = "Mượn sách thành công" };
        }
        catch (Exception ex)
        {
            return new BorrowResponse { Success = false, Message = ex.Message };
        }
    }

    public override async Task<ReturnResponse> ReturnBook(ReturnRequest request, ServerCallContext context)
    {
        try
        {
            var command = new ReturnBookCommand
            {
                DanhSachMaVachRFID = request.DanhSachMaVachRFID.ToList(),
                TinhTrangKiemTra = request.TinhTrangKiemTra
            };
            var result = await _mediator.Send(command);
            return new ReturnResponse { Success = result, Message = "Trả sách thành công" };
        }
        catch (Exception ex)
        {
            return new ReturnResponse { Success = false, Message = ex.Message };
        }
    }

    public override async Task<RenewResponse> RenewBook(RenewRequest request, ServerCallContext context)
    {
        try
        {
            var command = new RenewBookCommand
            {
                MaTheDocGia = request.MaTheDocGia,
                DanhSachMaVachRFID = request.DanhSachMaVachRFID.ToList()
            };
            var result = await _mediator.Send(command);
            return new RenewResponse { Success = result, Message = "Gia hạn sách thành công" };
        }
        catch (Exception ex)
        {
            return new RenewResponse { Success = false, Message = ex.Message };
        }
    }

    public override async Task<ReserveResponse> ReserveBook(ReserveRequest request, ServerCallContext context)
    {
        try
        {
            var command = new ReserveBookCommand
            {
                MaTheDocGia = request.MaTheDocGia,
                DauSachId = request.DauSachId
            };
            var result = await _mediator.Send(command);
            return new ReserveResponse { Success = result, Message = "Đặt trước sách thành công" };
        }
        catch (Exception ex)
        {
            return new ReserveResponse { Success = false, Message = ex.Message };
        }
    }

    public override async Task<PayFineResponse> PayFine(PayFineRequest request, ServerCallContext context)
    {
        try
        {
            var command = new PayFineCommand
            {
                MaTheDocGia = request.MaTheDocGia
            };
            var result = await _mediator.Send(command);
            return new PayFineResponse { Success = true, TongTienDaThu = result, Message = "Thu phạt thành công" };
        }
        catch (Exception ex)
        {
            return new PayFineResponse { Success = false, Message = ex.Message };
        }
    }

    public override async Task<GetBooksResponse> GetBooks(GetBooksRequest request, ServerCallContext context)
    {
        var results = await _mediator.Send(new LibraryManagement.Application.Features.Books.Queries.GetBooksQuery());
        var response = new GetBooksResponse();
        
        foreach(var item in results)
        {
            response.Books.Add(new BookItemResponse
            {
                MaVachRFID = item.MaVachRFID,
                TenSach = item.TenSach,
                Isbn = item.ISBN,
                TrangThai = item.TrangThai
            });
        }
        
        return response;
    }
}
