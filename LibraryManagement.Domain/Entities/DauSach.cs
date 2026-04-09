namespace LibraryManagement.Domain.Entities;

public class DauSach
{
    public string Id { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string TenSach { get; set; } = string.Empty;
    public string TacGia { get; set; } = string.Empty;
    public string TheLoai { get; set; } = string.Empty;
    public string NhaXuatBan { get; set; } = string.Empty;

    // Navigation properties
    public virtual ICollection<CuonSach> CuonSachs { get; set; } = new List<CuonSach>();
    public virtual ICollection<PhieuDatTruoc> PhieuDatTruocs { get; set; } = new List<PhieuDatTruoc>();

    public void KiemTraThongTin()
    {
        // Implementation logic
    }
}
