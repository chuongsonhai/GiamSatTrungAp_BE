using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.CMIS
{
    public class CdDoDdien
    {
        public CdDoDdien()
        {

        }
        public string MA_DVIQLY { get; set; }
        public string MA_DDO_DDIEN { get; set; }
        public string MA_YCAU_KNAI { get; set; }
        public string SO_NHA { get; set; }
        public string DUONG_PHO { get; set; }
        public int TINH_TRANG { get; set; } = 1;
        public string DINH_DANH { get; set; }
        public string ID_DIA_CHINH { get; set; }
        public string CONG_SUAT { get; set; }
        public string MDICH_SHOAT { get; set; }
        public string MDICH_CTIET { get; set; }
        public string SO_PHA { get; set; }
        public string LOAI_TRAM { get; set; }
        public int DTU_CTRINH { get; set; } = 0;
        public int SNGAY_YCAU { get; set; } = 1;
        public int SNGAY_ND { get; set; } = 1;
        public int SNGAY_KH { get; set; } = 0;
        public int TTRANG_DDIEN { get; set; } = 0;
        public string TNGUYEN_CSUAT { get; set; }
        public string SLUONG_MBA { get; set; }
        public string CSUAT_MBA { get; set; }
        public string TNGUYEN_CSUAT_MBA { get; set; }
        public int MA_CAPDA { get; set; } = 1;
        public string GHI_CHU { get; set; }
        public string NGAY_TAO { get; set; }
        public string NGUOI_TAO { get; set; }
        public string NGAY_SUA { get; set; }
        public string NGUOI_SUA { get; set; }
        public string MA_CNANG { get; set; }
        public string CSUAT_MTAM { get; set; }
    }
}
