namespace LibraryManagement.Domain.Entities;

public class ThuThu : NguoiDung
{
    public string CaLamViec { get; set; } = string.Empty;

    // Navigation properties
    public virtual ICollection<GiaoDichMuonTra> GiaoDichMuonTras { get; set; } = new List<GiaoDichMuonTra>();
    public virtual ICollection<PhieuPhat> PhieuPhats { get; set; } = new List<PhieuPhat>();

    public void TaoGiaoDich()
    {
        // Implementation logic
    }

    public void XuLyPhat()
    {
        // Implementation logic
    }
}
