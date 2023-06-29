namespace EVN.Core
{
    public class TienTiepNhanData
    {
        public virtual string MaLoaiYeuCau { get; set; }
        public virtual string MaYeuCau { get; set; }
        public virtual string MaDViTNhan { get; set; }
        public virtual string MaDViQLy { get; set; }
        public virtual string MaKHang { get; set; }
        public virtual string MaDDoDDien { get; set; }

        public virtual string NguoiYeuCau { get; set; }
        public virtual string DChiNguoiYeuCau { get; set; }
        public virtual string TenKhachHang { get; set; }
        public virtual string CoQuanChuQuan { get; set; }
        public virtual string DiaChiCoQuan { get; set; }
        public virtual string MST { get; set; }
        public virtual string DienThoai { get; set; }
        public virtual string Email { get; set; }
        public virtual string Fax { get; set; }
        public virtual string SoTaiKhoan { get; set; }
        public virtual string MaNHang { get; set; }

        public virtual string SoNha { get; set; }
        public virtual string DuongPho { get; set; }
        public virtual string DiaChinhID { get; set; }

        public virtual int SoPha { get; set; } = 3;
        public virtual bool DienSinhHoat { get; set; }

        public virtual string NgayHen { get; set; }
        public virtual string NgayHenKhaoSat { get; set; }
        public virtual string NoiDungYeuCau { get; set; }

        public virtual int TinhTrang { get; set; }
        public virtual string MaHinhThuc { get; set; }

        public virtual string NgayTao { get; set; }
        public virtual string NgaySua { get; set; }
    }
}
