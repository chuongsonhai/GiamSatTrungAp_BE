using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.CMISApi.Models
{
    public class HoSoTI
    {
        public string MA_TI { get; set; }
        public string SO_TI { get; set; }
        public string NAM_SX { get; set; }
        public string MA_CLOAI { get; set; }
        public int SO_HUU { get; set; } = 0;
        public string MA_DVI_SD { get; set; }
        public int SLAN_SDUNG { get; set; } = 0;
        public int TTRANG_KD { get; set; } = 0;
        public string MA_BDONG { get; set; }    
        public string NGAY_BDONG { get; set; }
        public string SO_BBAN { get; set; }
        public string MA_KHO { get; set; }
        public string MA_NVIEN { get; set; }
        public string NGAY_KDINH { get; set; }
        public string NGAY_TAO { get; set; }
        public string NGUOI_TAO { get; set; }
        public string NGAY_SUA { get; set; }
        public string NGUOI_SUA { get; set; }
        public string MA_CNANG { get; set; }
        public string NGAY_NHAP { get; set; }

        public string SO_HDONG { get; set; }
        public string SO_BBAN_KD { get; set; }
        public string MTEM_KD { get; set; }
        public string SERY_TEMKD { get; set; }
        public string MA_DVIKD { get; set; }
        public string MA_NVIENKD { get; set; }
        public string MCHI_KDINH { get; set; }
        public string SO_CHIKD { get; set; }

        public int TTHAI_INTTHAO { get; set; }
        public int TTRANG_CH { get; set; }
    }

    public class HoSoTU
    {
        public string MA_TI { get; set; }
        public string SO_TI { get; set; }
        public string NAM_SX { get; set; }
        public string MA_CLOAI { get; set; }
        public int SO_HUU { get; set; } = 0;
        public string MA_DVI_SD { get; set; }
        public int SLAN_SDUNG { get; set; } = 0;
        public int TTRANG_KD { get; set; } = 0;
        public string MA_BDONG { get; set; }
        public string NGAY_BDONG { get; set; }
        public string SO_BBAN { get; set; }
        public string MA_KHO { get; set; }
        public string MA_NVIEN { get; set; }
        public string NGAY_KDINH { get; set; }
        public string NGAY_TAO { get; set; }
        public string NGUOI_TAO { get; set; }
        public string NGAY_SUA { get; set; }
        public string NGUOI_SUA { get; set; }
        public string MA_CNANG { get; set; }
        public string NGAY_NHAP { get; set; }

        public string SO_HDONG { get; set; }
        public string SO_BBAN_KD { get; set; }
        public string MTEM_KD { get; set; }
        public string SERY_TEMKD { get; set; }
        public string MA_DVIKD { get; set; }
        public string MA_NVIENKD { get; set; }
        public string MCHI_KDINH { get; set; }
        public string SO_CHIKD { get; set; }

        public int TTHAI_INTTHAO { get; set; }
        public int TTRANG_CH { get; set; }
    }
}