using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class DanhMucLoaiCanhBao
    {
        public virtual int Id { get; set; }
        public virtual string TenLoaiCanhBao { get; set; }
        public virtual int ChuKyGui { get; set; }
        public virtual int PhanLoai { get; set; }
    }
}
