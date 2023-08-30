using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Models
{
   public class BaoCaoChiTietGiamSatTienDo
    {
        
        public virtual string MaDViQuanLy { get; set; }
        public virtual string MaYeuCau { get; set; }
        public virtual string TenKhachHang { get; set; }
        public virtual string DiaChi { get; set; }
        public virtual string SDT { get; set; }
        public virtual int TongCongSuatDangKy { get; set; }
        public virtual DateTime NgayTiepNhan { get; set; }
        public virtual int HangMucCanhBao { get; set; }
        public virtual string NguongCanhBao{ get; set; }
        public virtual int TrangThai { get; set; }
        public virtual DateTime NgayGioGiamSat { get; set; }
        public virtual string NguoiGiamSat { get; set; }
        public virtual string NoiDungKhaoSat { get; set; }
        public virtual string NoiDungXuLyYKienKH { get; set; }
        public virtual string PhanHoi { get; set; }
        public virtual string XacMinhNguyenNhanChamGiaiQuyet { get; set; }
        public virtual string NDGhiNhanVaChuyenDonViXuLy { get; set; }
        public virtual string KetQua{ get; set; }
    }
}
