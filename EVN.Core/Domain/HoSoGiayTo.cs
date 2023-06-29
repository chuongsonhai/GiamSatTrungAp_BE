using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class HoSoGiayTo
    {
        public virtual int ID { get; set; }
        public virtual string MaHoSo { get; set; } = Guid.NewGuid().ToString("N");
        public virtual string MaDViQLy { get; set; }
        public virtual string MaYeuCau { get; set; }
        public virtual string LoaiHoSo { get; set; }
        public virtual string TenHoSo { get; set; }
        public virtual string LoaiFile { get; set; } = "PDF";
        public virtual string Data { get; set; }
        public virtual int TrangThai { get; set; } = 0;//-1: Dự thảo, 0: Mới tạo, 1: KH ký, 2: EVN ký, 7: Xác nhận
        public virtual string NguoiKy { get; set; }
        public virtual string ChucVu { get; set; }
        public virtual DateTime NgayTao { get; set; } = DateTime.Now;
        public virtual bool KHXacNhan { get; set; } = false;
        public virtual bool TrinhKy { get; set; } = false;
    }    
}
