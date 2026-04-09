$OutputEncoding = [Console]::OutputEncoding = [Text.Encoding]::UTF8
$outputFile = "d:\Projects\LibraryManagement\docs\BaoCao_HoanChinh.md"

$content = @'
# TÀI LIỆU KỸ THUẬT: HỆ THỐNG QUẢN LÝ THƯ VIỆN

## I. Phân tích thiết kế

### Sơ đồ Cấu trúc Phụ thuộc (Dependency Diagram)
Sơ đồ dưới đây thể hiện "The Dependency Rule" (Quy tắc phụ thuộc hướng tâm) được áp dụng tuyệt đối trong dự án. Các mũi tên liền mạch thể hiện tham chiếu Project (Project Reference), mũi tên đứt nét thể hiện triển khai kỹ thuật (Interface Implementation & DI).

```mermaid
flowchart TD
    classDef core fill:#4CAF50,stroke:#388E3C,stroke-width:2px,color:white;
    classDef app fill:#2196F3,stroke:#1976D2,stroke-width:2px,color:white;
    classDef infra fill:#FF9800,stroke:#F57C00,stroke-width:2px,color:white;
    classDef pres fill:#9C27B0,stroke:#7B1FA2,stroke-width:2px,color:white;
    classDef web fill:#F44336,stroke:#D32F2F,stroke-width:2px,color:white;

    Web["🖥️ WebApp (Blazor Server UI)"]:::web
    Pres["🌐 Presentation (gRPC Service)"]:::pres
    Infra["🗄️ Infrastructure (Bộ nhớ RAM / Repositories)"]:::infra
    App["⚙️ Application (Use-Cases / CQRS)"]:::app
    Dom["💎 Domain (Core Entities & Exceptions)"]:::core

    Web == "Gọi qua luồng gRPC Client" ==> Pres
    Pres -. "Cấp phát Dependency Injection" .-> Infra
    Pres -- "Gửi Command / Query" --> App
    
    Infra -. "Triển khai Repository Interfaces" .-> App
    Infra -- "Lưu trữ / Thao tác" --> Dom
    
    App -- "Chứa Business Logic" --> Dom
```

---

## II. Triển khai hệ thống

### 1. Tổng quan Kiến trúc

Dự án Hệ thống Quản lý Thư viện được phát triển dựa trên mô hình Clean Architecture. Thiết kế này giúp định tuyến rõ ràng giữa logic nghiệp vụ và các khía cạnh kỹ thuật (cơ sở dữ liệu, giao diện, API giao tiếp).

Các mẫu thiết kế nổi bật được áp dụng bao gồm:
- CQRS (Command Query Responsibility Segregation) và thư viện MediatR để chia tách hệ thống xử lý Request.
- Repository Pattern và UnitOfWork để tập trung hóa thao tác truy xuất dữ liệu.
- In-Memory Data Store áp dụng `ConcurrentDictionary` để thao tác an toàn trong môi trường đa luồng cục bộ.
- Protocol Buffers và gRPC thực thi các giao tiếp nội bào tốc độ cao.

---

### 2. Quá trình Khởi tạo và Cấu hình Project

Dự án được khởi tạo thông qua .NET CLI. Các bước sau liệt kê quá trình lập cấu trúc source code cơ sở:

```bash
# 1. Tạo Solution
dotnet new sln -n LibraryManagement

# 2. Tạo 5 projects (tương ứng với các tầng)
dotnet new classlib -n LibraryManagement.Domain
dotnet new classlib -n LibraryManagement.Application
dotnet new classlib -n LibraryManagement.Infrastructure
dotnet new grpc -n LibraryManagement.Presentation
dotnet new blazorserver -n LibraryManagement.WebApp

# 3. Đưa các projects vào Solution
dotnet sln add LibraryManagement.Domain/LibraryManagement.Domain.csproj
dotnet sln add LibraryManagement.Application/LibraryManagement.Application.csproj
dotnet sln add LibraryManagement.Infrastructure/LibraryManagement.Infrastructure.csproj
dotnet sln add LibraryManagement.Presentation/LibraryManagement.Presentation.csproj
dotnet sln add LibraryManagement.WebApp/LibraryManagement.WebApp.csproj

# 4. Cấu hình Dependencies (áp dụng tham chiếu tâm)
dotnet add LibraryManagement.Application/LibraryManagement.Application.csproj reference LibraryManagement.Domain/LibraryManagement.Domain.csproj

dotnet add LibraryManagement.Infrastructure/LibraryManagement.Infrastructure.csproj reference LibraryManagement.Application/LibraryManagement.Application.csproj
dotnet add LibraryManagement.Infrastructure/LibraryManagement.Infrastructure.csproj reference LibraryManagement.Domain/LibraryManagement.Domain.csproj

dotnet add LibraryManagement.Presentation/LibraryManagement.Presentation.csproj reference LibraryManagement.Application/LibraryManagement.Application.csproj
dotnet add LibraryManagement.Presentation/LibraryManagement.Presentation.csproj reference LibraryManagement.Infrastructure/LibraryManagement.Infrastructure.csproj

# 5. Cài đặt các thư viện package mở rộng
dotnet add LibraryManagement.Application/LibraryManagement.Application.csproj package MediatR
dotnet add LibraryManagement.Presentation/LibraryManagement.Presentation.csproj package Grpc.AspNetCore
dotnet add LibraryManagement.WebApp/LibraryManagement.WebApp.csproj package Grpc.Net.Client

# 6. Thiết lập thư mục cơ bản
mkdir -p LibraryManagement.Application/Features/Borrowing/Commands
mkdir -p LibraryManagement.Application/Features/Borrowing/Queries
mkdir -p LibraryManagement.Application/Features/Borrowing/DTOs
mkdir -p LibraryManagement.Application/Interfaces

mkdir -p LibraryManagement.Domain/Entities
mkdir -p LibraryManagement.Domain/Enums
mkdir -p LibraryManagement.Domain/Exceptions

rm LibraryManagement.Domain/Class1.cs
rm LibraryManagement.Application/Class1.cs
rm LibraryManagement.Infrastructure/Class1.cs
```

---

### 3. Cấu trúc thư mục hệ thống

- **LibraryManagement.Domain**: Chứa định nghĩa các Entities, Enums, và Exceptions cốt lõi theo đặc tả nghiệp vụ.
- **LibraryManagement.Application**: Chứa định nghĩa các interface trừu tượng và triển khai Handler cho Use cases điều hướng bởi MediatR.
- **LibraryManagement.Infrastructure**: Tập trung logic cập nhật / truy vấn dữ liệu từ bộ nhớ ứng dụng.
- **LibraryManagement.Presentation**: gRPC endpoint phục vụ yêu cầu từ các client.
- **LibraryManagement.WebApp**: Hệ thống Web UI dựa vào SSR của Blazor Server.

---

### 4. Chi tiết triển khai và Source code

'@
[System.IO.File]::WriteAllText($outputFile, $content, [System.Text.Encoding]::UTF8)

function Add-Section {
    param([string]$title, [string]$desc, [string[]]$paths)
    [System.IO.File]::AppendAllText($outputFile, "#### $title`n`n", [System.Text.Encoding]::UTF8)
    [System.IO.File]::AppendAllText($outputFile, "$desc`n`n", [System.Text.Encoding]::UTF8)

    foreach ($path in $paths) {
        $files = Get-ChildItem -Path $path -File -Recurse -Include *.cs,*.razor,*.proto | Sort-Object Name
        foreach ($f in $files) {
            if ($f.FullName -match "\\bin\\" -or $f.FullName -match "\\obj\\" -or $f.FullName -match "GlobalUsings.g.cs" -or $f.FullName -match "\.g\.cs" -or $f.FullName -match "AssemblyAttributes") { continue }
            
            $ext = "csharp"
            if ($f.Extension -eq ".razor") { $ext = "html" }
            if ($f.Extension -eq ".proto") { $ext = "protobuf" }
            
            $relPath = $f.FullName -replace "(?i)^d:\\Projects\\LibraryManagement\\", ""
            
            $header = "> **File:** " + $relPath + "`n"
            [System.IO.File]::AppendAllText($outputFile, $header, [System.Text.Encoding]::UTF8)
            $startTag = '```' + $ext + "`n"
            [System.IO.File]::AppendAllText($outputFile, $startTag, [System.Text.Encoding]::UTF8)
            
            $content = [System.IO.File]::ReadAllText($f.FullName)
            [System.IO.File]::AppendAllText($outputFile, $content + "`n", [System.Text.Encoding]::UTF8)
            
            $endTag = '```' + "`n`n"
            [System.IO.File]::AppendAllText($outputFile, $endTag, [System.Text.Encoding]::UTF8)
        }
    }
}

$descDomain = @"
**1. Vai trò:**
Tầng Domain là cốt lõi của Kiến trúc Sạch (Clean Architecture). Tầng này hoàn toàn độc lập, không tham chiếu đến bất kỳ dự án hay thư viện ngoại vi nào (không EF Core, không ASP.NET). Điều này đảm bảo logic nghiệp vụ được bảo vệ khỏi sự thay đổi của công nghệ bên ngoài.

**2. Thành phần kỹ thuật:**
- **Entities (Thực thể):** Triển khai theo mô hình Rich Domain Model thay vì Anemic Domain Model. Các thực thể như `DocGia`, `CuonSach`, `PhieuPhat` không chỉ là các cấu trúc dữ liệu thuần túy (chứa properties getter/setter) mà còn chứa trọn vẹn logic tự thân. Ví dụ, `DocGia` có phương thức tự động quản trị số lượng sách đang mượn hoặc ném lỗi nếu thẻ hết hạn.
- **Exceptions (Ngoại lệ Nghiệp vụ):** Toàn bộ các quy tắc rào cản (Business Rules) được mô hình hóa thành Exception chuyên biệt như `LimitExceededException` hay `ReaderLockedException`.
- **Enums:** Chuẩn hóa các trạng thái tĩnh (Trạng thái sách, Trạng thái tài khoản).

**3. Cơ chế hoạt động:**
Bất kỳ thành phần nào muốn giao tiếp hoặc xoay chuyển trạng thái của dữ liệu đều phải đi qua các hàm được public của Entities, đảm bảo tính Đóng gói (Encapsulation) tuyệt đối.
"@

$descApp = @"
**1. Vai trò:**
Tầng Application định nghĩa và điều phối tất cả các Use Case (Luồng tính năng) của hệ thống quản lý thư viện. Tầng này chỉ phụ thuộc vào tầng Domain.

**2. Thành phần kỹ thuật:**
- **CQRS (Command Query Responsibility Segregation):** Mã nguồn được chia tách rõ rệt dựa trên hành vi:
  - `Commands`: Chịu trách nhiệm thay đổi trạng thái hệ thống (Mutate state) như Mượn sách (`BorrowBookCommand`), Trả sách, Thu phạt.
  - `Queries`: Chịu trách nhiệm chỉ múc dữ liệu phục vụ hiển thị (`GetBooksQuery`) mà không làm thay đổi state.
- **MediatR (Design Pattern Mediator):** Thay vì các module gọi chéo nhau tạo ra Dependency Hell, mọi truy vấn được gửi vào một đầu mối `IMediator`. Sau đó luồng được tự động phân bổ về đúng class `Handler` (ví dụ `BorrowBookCommandHandler`) giúp mã nguồn đạt độ quy chuẩn Single Responsibility Principle (SRP) tối đa.
- **Interfaces (Hợp đồng):** Tầng này định nghĩa các khuôn mẫu `IUnitOfWork`, `IRepository`... ứng dụng triệt để nguyên lý Dependency Inversion của SOLID. Hạ tầng phải nương theo Application chứ không phải chiều ngược lại.
"@

$descInfra = @"
**1. Vai trò:**
Tầng Infrastructure đóng vai trò tiếp hợp dữ liệu (Data Access Layer), ở đây thực thi toàn bộ các interface trừu tượng được đặt ra bởi tầng Application.

**2. Thành phần kỹ thuật:**
- **Bộ đệm In-Memory và ConcurrentDictionary:** Nhằm phô diễn giải pháp kiến trúc thay vì tốn kém tài nguyên cài SQL Server, dự án cấu hình bộ nhớ vật lý nội bộ (RAM). Mấu chốt kỹ thuật nằm ở việc ứng dụng `ConcurrentDictionary`. Khác với Dictionary tĩnh, kiến trúc này cung cấp khả năng Thread-safe, giải quyết dứt điểm rủi ro xung đột dữ liệu (Race condition) kể cả khi hàng loạt Request gRPC được đẩy vào cùng một mili-giây.
- **Repository Pattern:** Các thao tác truy vấn (Get), Lọc (Find), Cập nhật (UpdateAsync) được che giấu toàn bộ logic vòng lặp bên trong hệ thống `DocGiaRepository`, `CuonSachRepository`.
- **Unit Of Work:** Triển khai `InMemUnitOfWork` nhằm đảm bảo nguyên tắc ACID trong mọi giao dịch. Dữ liệu chỉ được commit thực sự nếu Use case chạy thành công hoàn toàn mà không gặp exception ở giữa chừng.
"@

$descAPI = @"
**1. Vai trò:**
Tầng Presentation đóng vai trò Gateway giao tiếp, mở cổng mạng lưới tiếp nhận Request từ tầng UI Client và chuyển dịch phản hồi. Thay vì thiết lập hàng chục Controller RESTful, hệ thống xây dựng điểm cuối bằng HTTP/2 dựa trên giao thức gRPC.

**2. Thành phần kỹ thuật:**
- **Protobuf (.proto):** Là trái tim của giao tiếp. File `Library.proto` quy định chặt chẽ toàn bộ Message Requests và Responses bằng kỹ thuật định kiểu an toàn (Strong-type) để mã hóa/giải mã thành dòng nhị phân, vượt trội hiệu năng so với chuỗi văn bản JSON.
- **Services (LibraryGrpcService.cs):** Class service duy nhất kế thừa quy chuẩn đã được dịch từ file `.proto`. Tại đây mã nguồn thể hiện tư tưởng tối giản (Thin Controller): Lớp này chỉ hứng dữ liệu chuyển đổi mạng, gửi thẳng Object model xuống Mediator của tầng Application và bắt Exception trả về RPC trả lời cho người thiết lập. Code không bao giờ chứa If-Else nghiệp vụ tại tầng này.
"@

$descUI = @"
**1. Vai trò:**
Tầng WebApp đem lại giao diện người dùng trọn vẹn để mô phỏng tương tác thu ngân, quản thư và độc giả. Hoàn toàn được tách biệt vật lý độc lập với Backend Core.

**2. Thành phần kỹ thuật:**
- **Blazor Server Framework:** Giao diện được xử trí bằng Server-side Rendering (SSR) nâng cao bằng C#, giao tiếp liên tục với DOM của trình duyệt thông qua hệ thống WebSocket (kênh SignalR) giúp thao tác được cập nhật Real-time.
- **gRPC Client Gateway:** Config bằng DI tại `Program.cs`, client truyền tải xác thực đến hệ thống Backend, bypass những thiết lập HTTP SSL cục bộ (qua `DangerousAcceptAnyServerCertificateValidator`) nhằm loại bỏ sự phức tạp tạo chứng chỉ nội bộ. Mọi thao tác tương tác giao diện (VD quét mã RFID) kết nối thẳng vào Backend RPC Service bằng chính cấu trúc Object định nghĩa của Protocol Buffers.
"@


Add-Section "Tầng Domain" $descDomain @("d:\Projects\LibraryManagement\LibraryManagement.Domain")
Add-Section "Tầng Application" $descApp @("d:\Projects\LibraryManagement\LibraryManagement.Application")
Add-Section "Tầng Infrastructure" $descInfra @("d:\Projects\LibraryManagement\LibraryManagement.Infrastructure")
Add-Section "Tầng Presentation (gRPC Server)" $descAPI @("d:\Projects\LibraryManagement\LibraryManagement.Presentation")
Add-Section "Tầng Web Application" $descUI @("d:\Projects\LibraryManagement\LibraryManagement.WebApp")

Write-Host "Hoan thanh lap BaoCao_HoanChinh.md"
