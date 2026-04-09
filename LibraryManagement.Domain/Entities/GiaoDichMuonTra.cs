using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Entities;

public class GiaoDichMuonTra
{
    public string Id { get; set; } = string.Empty; // Formerly MaGiaoDich
    public DateTime NgayMuon { get; set; }
    public DateTime NgayDenHan { get; set; }
    public DateTime? NgayTraThucTe { get; set; }
    public int SoLanGiaHan { get; set; }
    public TrangThaiGiaoDich TrangThaiGD { get; set; }

    // Foreign Keys
    public string DocGiaId { get; set; } = string.Empty; 
    public string MaThe { get; set; } = string.Empty; // Keep MaThe for existing code
    public virtual DocGia DocGia { get; set; } = null!;

    public string MaNguoiDungThuThu { get; set; } = string.Empty;
    public virtual ThuThu ThuThu { get; set; } = null!;

    public string MaVachRFID { get; set; } = string.Empty;
    public virtual CuonSach CuonSach { get; set; } = null!; // Actually, prompt said 1-N. Let's keep it 1-N but simplify mapping.

    // Navigation properties
    public virtual ICollection<CuonSach> CuonSachs { get; set; } = new List<CuonSach>();
    public virtual ICollection<PhieuPhat> PhieuPhats { get; set; } = new List<PhieuPhat>();

    public DateTime TinhNgayDenHan()
    {
        return NgayDenHan;
    }

    public bool KiemTraQuaHan()
    {
        return (NgayTraThucTe ?? DateTime.Now) > NgayDenHan;
    }

    public bool GiaHanSach()
    {
        if (SoLanGiaHan < 3) // Example limit
        {
            SoLanGiaHan++;
            NgayDenHan = NgayDenHan.AddDays(7);
            return true;
        }
        return false;
    }
}
