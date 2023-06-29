using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class ThoaThuanDNChiTiet
    {
        public virtual int ID { get; set; }        
        public virtual string MaYeuCau { get; set; }
        public virtual DateTime NgayYeuCau { get; set; } = DateTime.Now;
        public virtual string DuAnDien { get; set; }
        public virtual string DiaChiDungDien { get; set; }
        public virtual int TrangThai { get; set; } = 0;//0: Mới, 1: Đang xử lý, 2: Hoàn thành
        public virtual DoingBusinessType Type { get; set; } = DoingBusinessType.CongVan; //0: Hồ sơ hợp lệ, 1: Yêu cầu khảo sát, 2: Yêu cầu lập dự thảo thỏa thuận, 3: Hoàn thành, 7: Bị hủy
        public virtual string MoTa { get; set; }
        public virtual DateTime NgayThucHien { get; set; } = DateTime.Now;
        public virtual DateTime UpdateDate { get; set; } = DateTime.Now;
    }

    public enum DoingBusinessType
    {
        CongVan = 0,
        KhaoSat = 1,
        ThoaThuan = 2,
        HoanThanhTTDN = 3,
        KTDongDien = 4,
        HopDong = 5,
        NghiemThu = 6,
        Huy = 7
    }
}
