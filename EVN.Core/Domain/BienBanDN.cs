using EVN.Core.IServices;
using EVN.Core.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;

namespace EVN.Core.Domain
{
    public class BienBanDN
    {
        public virtual int ID { get; set; }

        public virtual string MaDViQLy { get; set; }
        public virtual string MaDViTNhan { get; set; }
        public virtual string MaLoaiYeuCau { get; set; } = "TBAC_D";

        public virtual string MaYeuCau { get; set; }
        public virtual string MaDDoDDien { get; set; }

        public virtual string SoCongVan { get; set; }
        public virtual DateTime NgayCongVan { get; set; } = DateTime.Now;

        public virtual string SoBienBanKS { get; set; }
        public virtual DateTime NgayKhaoSat { get; set; } = DateTime.Now;

        public virtual string MaKH { get; set; }
        
        public virtual string SoBienBan { get; set; }
        public virtual DateTime NgayBienBan { get; set; } = DateTime.Now;

        public virtual string EVNDonVi { get; set; }
        public virtual string EVNDaiDien { get; set; }
        public virtual string EVNChucVu { get; set; }
        public virtual string EVNDiaChi { get; set; }
        public virtual string EVNDienThoai { get; set; }
        public virtual string EVNFax { get; set; }
        public virtual string EVNTaiKhoan { get; set; }
        public virtual string EVNMaSoThue { get; set; }

        public virtual string KHTen { get; set; }
        public virtual string KHDaiDien { get; set; }
        public virtual string KHChucDanh { get; set; }
        public virtual string KHDiaChi { get; set; }
        public virtual string KHDienThoai { get; set; }
        public virtual string KHTaiKhoan { get; set; }
        public virtual string KHMaSoThue { get; set; }

        public virtual string TenCongTrinh { get; set; }
        public virtual string DiaDiemXayDung { get; set; }
        public virtual string NoiDung { get; set; }
        public virtual string TaiLieuDinhKem { get; set; }

        public virtual string ThoaThuanKhac { get; set; }
        public virtual string HTDoDem { get; set; }
        public virtual string RanhGioiDauTu { get; set; }
        public virtual string YeuCauKyThuat { get; set; }

        public virtual string NgayDauNoi { get; set; }
        public virtual DateTime NgayLap { get; set; } = DateTime.Now;
        public virtual string NguoiLap { get; set; }
        public virtual DateTime NgayDuyet { get; set; } = DateTime.Now;
        public virtual string NguoiDuyet { get; set; }
        public virtual DateTime NgayKy { get; set; } = DateTime.Now;
        public virtual string NguoiKy { get; set; }
        public virtual int TrangThai { get; set; } = -1;
        public virtual string Data { get; set; }
        public virtual string DocPath { get; set; }

        public virtual string MaCViec { get; set; }

        public virtual string SoThongTu { get; set; } = "39/2015/TT-BCT";
        public virtual DateTime NgayThongTu { get; set; } = new DateTime(2015, 11, 18);
        public virtual string TroNgai { get; set; }
        public virtual bool KHXacNhan { get; set; } = false;        
    }
}