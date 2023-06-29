using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class HSKemTheo
    {
        public virtual int ID { get; set; }
        public virtual string MaHoSo { get; set; } = Guid.NewGuid().ToString("N");
        public virtual string MaDViQLy { get; set; }
        public virtual string MaYeuCau { get; set; }
        public virtual string LoaiHoSo { get; set; }
        public virtual string TenHoSo { get; set; }
        public virtual string Data { get; set; }
        public virtual int TrangThai { get; set; } = 0;//0: Mới tạo, 1: KH ký, 2: EVN ký 
        public virtual int Type { get; set; } = 0;//0: TTDN,1:BBKT,2:BBNT, 3: Phụ lục hợp đồng, 4 : TLKS
    }    
}
