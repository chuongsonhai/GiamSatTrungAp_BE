using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class Organization
    {
        public virtual long orgId { get; set; }
        public virtual string orgCode { get; set; }
        public virtual string compCode { get; set; }
        public virtual string orgName { get; set; }
        public virtual string parentCode { get; set; }
        public virtual int capDvi { get; set; }
        public virtual string address { get; set; }
        public virtual int idDiaChinh { get; set; }

        public virtual DateTime updatetime { get; set; } = DateTime.Now;

        public virtual string phone { get; set; }
        public virtual string fax { get; set; }
        public virtual string email { get; set; }
        public virtual string daiDien { get; set; }
        public virtual string chucVu { get; set; }
        public virtual string taxCode { get; set; }
        public virtual string soTaiKhoan { get; set; }
        public virtual string nganHang { get; set; }

        public virtual int type { get; set; } = 0;
    }
}
