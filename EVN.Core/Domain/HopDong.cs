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
    public class HopDong
    {
        public virtual int ID { get; set; }

        public virtual string MaDViQLy { get; set; }
        public virtual string MaDViTNhan { get; set; }
        public virtual string MaYeuCau { get; set; }
        
        public virtual DateTime NgayHopDong { get; set; } = DateTime.Now;

        public virtual string DonVi { get; set; }
        public virtual string MaSoThue { get; set; }
        public virtual string DaiDien { get; set; }
        public virtual string ChucVu { get; set; }
        public virtual string DiaChi { get; set; }
        public virtual string DienThoai { get; set; }
        public virtual string Email { get; set; }

        public virtual string KHMa { get; set; }
        public virtual string KHTen { get; set; }
        public virtual string KHDaiDien { get; set; }
        public virtual string KHDiaChi { get; set; }
        public virtual string KHMaSoThue { get; set; }
        public virtual string KHDienThoai { get; set; }
        public virtual string KHEmail { get; set; }

        public virtual DateTime NgayXacNhan { get; set; } = DateTime.Now;

        public virtual int TrangThai { get; set; } = 0; //0: Đã tạo hợp đồng trên CMIS, 1: Gửi khách hàng kiểm tra, 2: Đã ký khách hàng, 3: Hoàn thành
        public virtual string Data { get; set; }

        public virtual string NoiDungXuLy { get; set; }

        public virtual bool DienSinhHoat { get; set; } = false;
    }
}