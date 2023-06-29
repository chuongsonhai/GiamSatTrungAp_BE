using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.CMISApi.Models
{
    public class DonViQLyData
    {
        public virtual string orgCode { get; set; }
        public virtual string orgName { get; set; }
        public virtual string parentCode { get; set; }
        public virtual int capDvi { get; set; }
        public virtual string address { get; set; }
        public virtual int idDiaChinh { get; set; }

        public virtual string phone { get; set; }
        public virtual string fax { get; set; }
        public virtual string email { get; set; }

        public virtual string taxCode { get; set; }

        public virtual string daiDien { get; set; }
        public virtual string chucVu { get; set; }
        
        public virtual string soTaiKhoan { get; set; }
        public virtual string nganHang { get; set; }
    }

    public class BoPhanData
    {
        public virtual string MA_DVIQLY { get; set; }
        public virtual string MA_BPHAN { get; set; }
        public virtual string TEN_BPHAN { get; set; }
        public virtual string MO_TA { get; set; }
        public virtual string GHI_CHU { get; set; }
    }

    public class NhanVienData
    {
        public virtual string MA_DVIQLY { get; set; }
        public virtual string MA_NVIEN { get; set; }
        public virtual string MA_BPHAN { get; set; }
        public virtual string TEN_NVIEN { get; set; }
    }
}