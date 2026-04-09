using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Entities;

public class DocGia : NguoiDung
{
    public string MaThe { get; set; } = string.Empty;
    public string LoaiThe { get; set; } = string.Empty;
    public DateTime NgayHetHanThe { get; set; }
    public int SoSachDangMuon { get; set; }
    public double TongNoPhat { get; set; }
    public TrangThaiTaiKhoan TrangThaiTaiKhoan { get; set; }

    // Navigation properties
    public virtual ICollection<PhieuDatTruoc> PhieuDatTruocs { get; set; } = new List<PhieuDatTruoc>();
    public virtual ICollection<GiaoDichMuonTra> GiaoDichMuonTras { get; set; } = new List<GiaoDichMuonTra>();
    public virtual ICollection<PhieuPhat> PhieuPhats { get; set; } = new List<PhieuPhat>();

    public bool KiemTraHopLe()
    {
        return NgayHetHanThe > DateTime.Now && TrangThaiTaiKhoan == TrangThaiTaiKhoan.BinhThuong;
    }

    public void CapNhatSoSachMuon(int soLuong)
    {
        SoSachDangMuon += soLuong;
    }
}
