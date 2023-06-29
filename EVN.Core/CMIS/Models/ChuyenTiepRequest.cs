using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.CMIS
{
    public class ChuyenTiepRequest
    {
        public string MA_DVIQLY { get; set; }
        public string CHUYEN_CAPDUOI { get; set; }
        public string MA_YCAU_KNAI { get; set; }
        public DvYeuCau DV_YEU_CAU { get; set; }
        public IList<TienTrinh> DV_TIEN_TRINH { get; set; } = new List<TienTrinh>();
        public DvTienTNhan DV_TIEN_TNHAN { get; set; }
        public IList<CdDoDdien> CD_DDO_DDIEN { get; set; } = new List<CdDoDdien>();
        public JArray CD_BKE_CSUAT_TBI { get; set; } = new JArray();
        public JArray CD_GTO_HDCHUNG { get; set; } = new JArray();
        public JArray CD_HO_DCHUNG { get; set; } = new JArray();
        public IList<KHangLienHe> CD_KHANG_LIENHE { get; set; } = new List<KHangLienHe>();
        public IList<HSoGiayTo> DV_HSO_GTO { get; set; } = new List<HSoGiayTo>();
    }
}
