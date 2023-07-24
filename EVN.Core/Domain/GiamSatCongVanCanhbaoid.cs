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
    public class GiamSatCongVanCanhbaoid
    {
        public virtual string MaYeuCau { get; set; }
        public virtual string TenKhachHang { get; set; }
        public virtual string DienThoai { get; set; }
        public virtual TrangThaiCongVan TrangThai { get; set; } = TrangThaiCongVan.MoiTao;
        //public virtual int TinhTrang { get; set; }
        public virtual int ID { get; set; }
        //public virtual string MaKHang { get; set; }
        //public virtual string MaDDoDDien { get; set; }

        //public virtual string SoCongVan { get; set; }        
      
        //public virtual DateTime NgayYeuCau { get; set; } = DateTime.Now;
        //public virtual string MaLoaiYeuCau { get; set; }

        //public virtual string MaDViTNhan { get; set; }
        //public virtual string MaDViQLy { get; set; }  
        //public virtual string BenNhan { get; set; }

        //public virtual string NguoiYeuCau { get; set; }
        //public virtual string DChiNguoiYeuCau { get; set; }        
        //public virtual string DuongPho { get; set; }
        //public virtual string SoNha { get; set; }
        //public virtual string DiaChinhID { get; set; } = "0";

    
        //public virtual string CoQuanChuQuan { get; set; }
        //public virtual string DiaChiCoQuan { get; set; }
        //public virtual string MST { get; set; }

        //public virtual string Email { get; set; }
        //public virtual string Fax { get; set; }
        //public virtual string SoTaiKhoan { get; set; }
        //public virtual string SoPha { get; set; } = "1";
        //public virtual bool DienSinhHoat { get; set; }

        //public virtual DateTime? NgayHen { get; set; } = DateTime.Now;
        //public virtual DateTime? NgayHenKhaoSat { get; set; } = DateTime.Now;


        //public virtual string MaHinhThuc { get; set; }
        
        //public virtual string NoiDungYeuCau { get; set; }

        //public virtual string DiaChiDungDien { get; set; }
        //public virtual string DuAnDien { get; set; }

        //public virtual int LoaiHinh { get; set; }
    
        
        //public virtual DateTime NgayLap { get; set; } = DateTime.Now;
        //public virtual string NguoiLap { get; set; }
        //public virtual string NguoiDuyet { get; set; }
        //public virtual DateTime NgayDuyet { get; set; } = DateTime.Now;
        //public virtual string Fkey { get; set; } = Guid.NewGuid().ToString();
        //public virtual string Data { get; set; }
        //public virtual string MaCViec { get; set; } = "YCM";

        //public virtual string LyDoHuy { get; set; }
        //public virtual string TenKhachHangUQ { get; set; }
        //public virtual string SoDienThoaiKHUQ { get; set; }

     
    }    
}