using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Entities;

public class CuonSach
{
    public string Id { get; set; } = string.Empty;
    public string MaVachRFID { get; set; } = string.Empty;
    public TinhTrangVatLy TinhTrangVatLy { get; set; }
    public TrangThaiCuonSach TrangThai { get; set; }
    
    // Foreign Key
    public string DauSachId { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty; // Keep ISBN for backward compatibility with existing code
    public virtual DauSach DauSach { get; set; } = null!;

    // Navigation properties
    public virtual ICollection<GiaoDichMuonTra> GiaoDichMuonTras { get; set; } = new List<GiaoDichMuonTra>();
}
