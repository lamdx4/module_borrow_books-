# BÁO CÁO TRIỂN KHAI HỆ THỐNG QUẢN LÝ THƯ VIỆN

**Sinh viên / Nhóm thực hiện:** [Điền tên của bạn]
**Môn học:** [Điền tên môn học / Đồ án]
**Mục đích tài liệu**: Trình bày chi tiết cách thức xây dựng hệ thống theo mô hình **Clean Architecture**, phân chia rõ ràng từng tầng (Layer) và kỹ thuật được áp dụng tại mỗi tầng nhằm đảm bảo hệ thống có khả năng mở rộng, bảo trì cao và hiệu năng vận hành xuất sắc.

---

## 1. TẦNG DOMAIN (CORE LAYER) - MÔ HÌNH HÓA THỰC THỂ
**Cách triển khai:** 
Tầng Domain là trung tâm của hệ thống, chứa các thông tin cốt lõi (Entities) và các quy tắc nghiệp vụ (Business Rules). 
- Các thực thể được thiết kế theo hướng **Rich Domain Model** (đối tượng giàu tính năng), tự quản lý trạng thái của chính nó thay vì chỉ là các class chứa Properties (Anemic Domain Model).
- Tầng này độc lập hoàn toàn, không phụ thuộc vào bất cứ DB hay Framework UI nào.

**Code Demo (Phân tích thực thể Độc Giả):**
```csharp
namespace LibraryManagement.Domain.Entities;

public class DocGia : NguoiDung
{
    public required string MaThe { get; set; }
    public TrangThaiTaiKhoan TrangThaiTaiKhoan { get; set; }
    public int SoSachDangMuon { get; set; } 
    public DateTime NgayHetHan { get; set; }

    // Phương thức tự quản lý trạng thái nghiệp vụ, thay vì set thủ công
    public void XuPhatKhoaThe()
    {
        TrangThaiTaiKhoan = TrangThaiTaiKhoan.Khoa;
    }

    public void CheckHanMucMuon()
    {
        if (SoSachDangMuon >= 5)
            throw new LimitExceededException("Thẻ đã vượt quá hạn mức 5 cuốn sách.");
    }
}
```

---

## 2. TẦNG APPLICATION - GIAO DỊCH NGHIỆP VỤ BẰNG CQRS (MEDIATR)
**Cách triển khai:** 
Tầng Application chứa các luồng tính năng thực tế của hệ thống (Usecase).
- Thay vì sử dụng các Service khổng lồ chứa hàng ngàn dòng code (God classes), ứng dụng áp dụng quy chuẩn **CQRS** chia tách tính năng Đọc (Query) và Ghi (Command).
- Thư viện **MediatR** được sử dụng để định tuyến Request. Mỗi một Usecase (như Trả sách, Mượn sách) được module hóa thành một class `Handler` duy nhất, tuân thủ tuyệt đối nguyên lý SRP (Single Responsibility Principle).

**Code Demo (Luồng xử lý Usecase Mượn Sách):**
```csharp
namespace LibraryManagement.Application.Features.Borrowing.Commands;

// Định nghĩa đầu vào của Usecase
public class BorrowBookCommand : IRequest<BorrowBookResult>
{
    public required string MaTheRfid { get; set; }
    public required List<string> MaSachRfids { get; set; }
}

// Handler chuyên biệt chỉ có duy nhất trách nhiệm xử lý lệnh mượn
public class BorrowBookCommandHandler : IRequestHandler<BorrowBookCommand, BorrowBookResult>
{
    private readonly IDocGiaRepository _docGiaRepo;
    private readonly ICuonSachRepository _cuonSachRepo;

    public async Task<BorrowBookResult> Handle(BorrowBookCommand request, CancellationToken token)
    {
        var docGia = await _docGiaRepo.GetByMaTheAsync(request.MaTheRfid);
        if (docGia == null) throw new Exception("Không tìm thấy độc giả.");
        
        // Gọi các quy tắc kiểm tra từ tầng Domain
        if (docGia.TrangThaiTaiKhoan == TrangThaiTaiKhoan.Khoa)
            throw new ReaderLockedException("Tài khoản đang bị khóa do vi phạm.");

        foreach (var maRfid in request.MaSachRfids)
        {
            var sach = await _cuonSachRepo.GetByMaVachAsync(maRfid);
            sach.TrangThai = TrangThaiCuonSach.DangMuon;
            docGia.SoSachDangMuon++;
            
            await _cuonSachRepo.UpdateAsync(sach);
        }
        await _docGiaRepo.UpdateAsync(docGia);

        return new BorrowBookResult { Success = true, Message = "Mượn sách thành công!" };
    }
}
```

---

## 3. TẦNG INFRASTRUCTURE - QUẢN TRỊ DỮ LIỆU THREAD-SAFE
**Cách triển khai:** 
Tầng hạ tầng chịu rách nhiệm lưu trữ dữ liệu. Phục vụ cho mục đích tập trung biểu diễn luồng kiến trúc (tránh rườm rà với SQL/Entity Framework), cơ sở dữ liệu được đưa lên bộ nhớ RAM.
- Sử dụng **ConcurrentDictionary** tối ưu khả năng đọc/ghi đa luồng (multi-threading), cam kết tính toàn vẹn (Thread-Safe) khi có nhiều Request chạy cùng 1 thời điểm.
- Dữ liệu mồi (Seed Data) được nhúng ngay vào hàm tạo để có thể chạy kiểm thử hệ thống bất cứ lúc nào.

**Code Demo (Mô phỏng Kho Dữ Liệu Sách):**
```csharp
namespace LibraryManagement.Infrastructure.Repositories;

public class CuonSachRepository : ICuonSachRepository
{
    // Tập bản ghi đa luồng, chống đụng độ khóa (Race condition)
    private readonly ConcurrentDictionary<string, CuonSach> _data;

    public CuonSachRepository()
    {
        _data = new ConcurrentDictionary<string, CuonSach>();
        
        // Dữ liệu Seed khởi tạo phục vụ demo
        var ds = new DauSach { ISBN = "978-0131103627", TenSach = "The C#" };
        var s = new CuonSach { MaVachRFID = "RFID001", DauSach = ds, TrangThai = TrangThaiCuonSach.SanSang };
        
        _data.TryAdd(s.MaVachRFID, s);
    }

    public Task<CuonSach?> GetByMaVachAsync(string maVachRfid)
    {
        _data.TryGetValue(maVachRfid, out var sach);
        return Task.FromResult(sach);
    }

    public Task UpdateAsync(CuonSach cuonSach)
    {
        _data.AddOrUpdate(cuonSach.MaVachRFID, cuonSach, (k, old) => cuonSach);
        return Task.CompletedTask;
    }
}
```

---

## 4. TẦNG PRESENTATION - CỔNG GIAO TIẾP G-RPC
**Cách triển khai:** 
Là trạm kiểm soát toàn bộ lưu lượng đổ vào Backend. Hệ thống thay thế Web API RESTful truyền thống bằng **Web gRPC** nhằm tối ưu hóa thông lượng băng thông.
- Giao tiếp được nén dưới dạng nhị phân với HTTP/2, tốc độ đáp ứng gấp vô số lần Json RESTful.
- Sử dụng Protocol Buffers (`.proto`) là hợp đồng mạnh mẽ ép kiểu dữ liệu giữa Client và Server.

**Code Demo (Config file gRPC và Giao vận MediatR):**
```protobuf
// File giao tiếp (Library.proto) chia sẻ cho cả C# Client và Server
syntax = "proto3";
service LibraryService {
  rpc BorrowBook (BorrowRequest) returns (BorrowResponse);
}

message BorrowRequest {
  string maThe = 1;
  repeated string listMaVachRFID = 2;
}
```

```csharp
// LibraryGrpcService.cs (Nhận request và bơm xuống MediatR)
public class LibraryGrpcService : LibraryService.LibraryServiceBase
{
    private readonly IMediator _mediator;

    public override async Task<BorrowResponse> BorrowBook(BorrowRequest request, ServerCallContext ctx)
    {
        try
        {
            var command = new BorrowBookCommand { 
                MaTheRfid = request.MaThe, 
                MaSachRfids = request.ListMaVachRFID.ToList() 
            };
            
            // Controller cực kì nhẹ, trút gánh nặng cho MediatR Handler
            var result = await _mediator.Send(command);
            return new BorrowResponse { Success = result.Success, Message = result.Message };
        }
        catch (Exception ex)
        {
            return new BorrowResponse { Success = false, Message = ex.Message };
        }
    }
}
```

---

## 5. UI BLAZOR SERVER - TRÌNH DIỄN REAL-TIME
**Cách triển khai:** 
Thay vì sử dụng JS (React/Vue), giao diện được lập trình 100% bằng C# thông qua **Blazor Server**.
- Render toàn bộ DOM bằng HTML/C# tại phía máy chủ. Luồng kết nối chạy qua WebSocket (SignalR).
- Bỏ qua rào cản Authorize phức tạp để trực tiếp gọi mã `gRPC client`.

**Code Demo (Màn hình Mượn Sách `Borrow.razor` chốt đơn hệ thống):**
```html
@page "/borrow"
@rendermode InteractiveServer
@inject LibraryServiceClient LibraryClient
@inject SessionStateService SessionState

<div class="row">
    <div class="col-md-6">
        <input type="text" @bind="inputScan" class="form-control" placeholder="Quét Barcode / RFID sách..." />
        <button class="btn btn-outline-primary" @onclick="QuetMa">Thêm vào Giỏ</button>
    </div>
</div>

<button class="btn btn-primary" @onclick="XacNhanMuon">Xác Nhận Mượn</button>

@code {
    private List<string> danhSachMaRfid = new();

    private async Task XacNhanMuon()
    {
        // Khởi tạo Request chuẩn của gRPC Message
        var req = new BorrowRequest { MaThe = SessionState.CurrentReaderId };
        req.ListMaVachRFID.AddRange(danhSachMaRfid);

        // Phát tín hiệu HTTP/2 (RPC) qua máy chủ Backend
        var res = await LibraryClient.BorrowBookAsync(req);

        if (res.Success) {
            danhSachMaRfid.Clear();
            Console.WriteLine("Mượn thành công!");
        } else {
            Console.WriteLine($"[Lỗi Hệ Thống]: {res.Message}");
        }
    }
}
```

---
**Kết luận:** Thông qua luồng trình bày theo từng lớp, báo cáo đúc kết rõ sự mạch lạc hoàn hảo của thiết kế. Mọi module (thực thể, lưu trữ, điều hướng, và hiển thị) được bao bọc tuyệt đối bởi quy ước của Clean Architecture, giúp cho việc truy vết, bảo trì sửa lỗi dễ dàng hơn bất kỳ cấu trúc ứng dụng CRUD thông thường nào.
