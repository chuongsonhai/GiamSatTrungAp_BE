using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.CMIS
{
    public class TiepNhanRequest
    {
        public string MA_DVIQLY { get; set; }
        public string MA_YCAU_KNAI { get; set; }
        public string MA_KHANG { get; set; }
    }

    public class TienDoRequest
    {
        public string MA_DVIQLY { get; set; }
        public string MA_YCAU_KNAI { get; set; }
        public string MA_DDO_DDIEN { get; set; }
    }

    public class TiepNhanResponse
    {
        public DvYeuCau bangDvYeuCau { get; set; }
        public DvTienTNhan bangDvTienTnhan { get; set; }
        public IList<TienTrinh> bangDvTienTrinh { get; set; } = new List<TienTrinh>();
        public JArray bangCdBkeCsuatTbi { get; set; } = new JArray();
        public IList<CdDoDdien> bangCdDoDdien { get; set; }
        public JObject bangCdDiemDo { get; set; } = new JObject();
        public JArray bangCdGtoHdchung { get; set; } = new JArray();
        public JArray bangCdHoDchung { get; set; } = new JArray();
        public IList<KHangLienHe> bangCdKhangLienhe { get; set; } = new List<KHangLienHe>();
        public IList<HSoGiayTo> bangDvHsoGto { get; set; } = new List<HSoGiayTo>();
        public JArray bangDvXminhCdan { get; set; } = new JArray();
        public JArray bangDvXminhCdan_Hdc { get; set; } = new JArray();
        public TTinCDan thongTinCDan_YC { get; set; } = new TTinCDan();
        public TTinCDan thongTinCDan_KH { get; set; } = new TTinCDan();
    }
}
