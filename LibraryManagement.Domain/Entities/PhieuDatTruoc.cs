namespace LibraryManagement.Domain.Entities;

public class PhieuDatTruoc
{
    public string Id { get; set; } = string.Empty;
    public DateTime NgayDat { get; set; }
    public DateTime NgayHetHanNhan { get; set; }
    public string TrangThai { get; set; } = string.Empty;

    // Foreign Keys
    public string MaThe { get; set; } = string.Empty;
    public virtual DocGia DocGia { get; set; } = null!;

    public string ISBN { get; set; } = string.Empty;
    public virtual DauSach DauSach { get; set; } = null!;

    public void GuiThongBao()
    {
        // Implementation logic
    }

    public void HuyDatTruoc()
    {
        // Implementation logic
    }
}
