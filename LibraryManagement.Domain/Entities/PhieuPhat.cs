using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Entities;

public class PhieuPhat
{
    public string Id { get; set; } = string.Empty; // Formerly MaPhieuPhat
    public string LyDoPhat { get; set; } = string.Empty;
    public double SoTienPhat { get; set; }
    public TrangThaiThanhToan TrangThaiThanhToan { get; set; }
    public DateTime NgayLapPhieu { get; set; }

    // Foreign Keys
    public string DocGiaId { get; set; } = string.Empty;
    public string MaThe { get; set; } = string.Empty; // Keep for existing code
    public virtual DocGia DocGia { get; set; } = null!;

    public string MaNguoiDungThuThu { get; set; } = string.Empty;
    public virtual ThuThu ThuThu { get; set; } = null!;

    public string GiaoDichMuonTraId { get; set; } = string.Empty;
    public virtual GiaoDichMuonTra GiaoDichMuonTra { get; set; } = null!;

    public double TinhTienPhat()
    {
        return SoTienPhat;
    }

    public void XacNhanThanhToan()
    {
        TrangThaiThanhToan = TrangThaiThanhToan.DaThanhToan;
    }
}
