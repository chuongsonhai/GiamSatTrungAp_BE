using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class BienBanTT
    {
        public virtual int ID { get; set; }
        public virtual string MA_DVIQLY { get; set; }
        public virtual string MA_YCAU_KNAI { get; set; }
        public virtual string TEN_CTY { get; set; }
        public virtual string TEN_DLUC { get; set; }
        public virtual string SO_BB { get; set; }
        public virtual string LY_DO { get; set; }
        public virtual string MA_LDO { get; set; }
        public virtual string MO_TA { get; set; }
        public virtual string TEN_KHACHHANG { get; set; }
        public virtual string SDT_KHACHHANG { get; set; }
        public virtual string NGUOI_DDIEN { get; set; }
        public virtual string DIA_DIEM { get; set; }
        public virtual string MA_DDO { get; set; }
        public virtual string MA_TRAM { get; set; }
        public virtual string MA_GCS { get; set; }
        public virtual string VTRI_LDAT { get; set; }
        public virtual string NVIEN_TTHAO { get; set; }
        public virtual string NVIEN_TTHAO2 { get; set; }
        public virtual string NVIEN_TTHAO3 { get; set; }
        public virtual string NVIEN_NPHONG { get; set; }
        public virtual DateTime NGAY_TAO { get; set; } = DateTime.Now;
        public virtual string NGUOI_TAO { get; set; }
        public virtual int TRANG_THAI { get; set; } = 0;
        public virtual string SO_COT { get; set; }
        public virtual string SO_HOP { get; set; }
        public virtual string Data { get; set; }
        public virtual string NoiDungXuLy { get; set; }

        public virtual bool KyNVTT { get; set; } = false;
        public virtual bool KyNVNP { get; set; } = false;

        public virtual IList<CongTo> CongTos { get; set; } = new List<CongTo>();
        public virtual IList<MayBienDienAp> MayBienDienAps { get; set; } = new List<MayBienDienAp>();
        public virtual IList<MayBienDong> MayBienDongs { get; set; } = new List<MayBienDong>();
    }
}