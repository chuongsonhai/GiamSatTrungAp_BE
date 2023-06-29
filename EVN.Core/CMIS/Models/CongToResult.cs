using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.CMIS
{
    public class CongToResult
    {
        public string SO_PHA { get; set; }
        public string MA_KHO { get; set; }
        public string NGAY_BDONG { get; set; }
        public string TEN_NVKD { get; set; }
        public string MA_NVIENKD { get; set; }
        public string DIEN_AP { get; set; }
        public string MTEM_KD { get; set; }
        public string TEM_KD { get; set; }
        public string NGAY_KDINH { get; set; }
        public string MA_BDONG { get; set; }
        public string MA_DVIKD { get; set; }
        public string NAM_SX { get; set; }
        public string DONG_DIEN { get; set; }
        public string NGAY_LTRINH { get; set; }
        public string MA_CLOAI { get; set; }
        public string VH_CONG { get; set; }
        public string SO_HUU { get; set; }
        public string SERY_TEMKD { get; set; }
        public string KIM_CHITAI { get; set; }
        public string LOAI_SOHUU { get; set; }
        public string LOAI_CTO { get; set; }
        public string SLAN_LT { get; set; }
        public string SO_CHITAI { get; set; }
        public string TYSO_TI { get; set; }
        public string BCS { get; set; }
        public string MA_CTO { get; set; }
        public string NGAY_NHAP { get; set; }
        public string SO_BBAN_KD { get; set; }
        public string SO_CTO { get; set; }
        public string MA_DVI_SD { get; set; }
        public string SO_BBAN { get; set; }
        public string TYSO_TU { get; set; }
    }

    public class TIResult
    {
        public string SO_PHA { get; set; }
        public string MA_KHO { get; set; }
        public string SO_HUU { get; set; }
        public string SERY_TEMKD { get; set; }
        public string NGAY_BDONG { get; set; }
        public string TEN_NVKD { get; set; }
        public string LOAI_SOHUU { get; set; }
        public string MA_NVIENKD { get; set; }
        public string DIEN_AP { get; set; }
        public string MTEM_KD { get; set; }
        public string TEM_KD { get; set; }
        public string TYSO_DAU { get; set; }
        public string NGAY_KDINH { get; set; }
        public string MA_BDONG { get; set; }
        public string MA_CTO { get; set; }
        public string NGAY_NHAP { get; set; }
        public string MA_DVIKD { get; set; }
        public string NAM_SX { get; set; }
        public string SO_BBAN_KD { get; set; }
        public string SO_CTO { get; set; }
        public string MA_CLOAI { get; set; }
        public string MA_DVI_SD { get; set; }
        public string SO_BBAN { get; set; }
    }

    public class TUResult
    {
        public string SO_PHA { get; set; }
        public string MA_KHO { get; set; }
        public string SO_HUU { get; set; }
        public string NGAY_BDONG { get; set; }
        public string LOAI_SOHUU { get; set; }
        public string DIEN_AP { get; set; }
        public string TYSO_DAU { get; set; }
        public string NGAY_KDINH { get; set; }
        public string MA_BDONG { get; set; }
        public string MA_CTO { get; set; }
        public string NGAY_NHAP { get; set; }
        public string NAM_SX { get; set; }
        public string SO_BBAN_KD { get; set; }
        public string SO_CTO { get; set; }
        public string MA_CLOAI { get; set; }
        public string MA_DVI_SD { get; set; }
        public string SO_BBAN { get; set; }
    }

    public class TTRamResult
    {
        public int csuatTram { get; set; }
        public string loaiTram { get; set; }
        public string maCapda { get; set; }
        public string maCapdaRa { get; set; }
        public string maCnang { get; set; }
        public string maDviqly { get; set; }
        public string maTo { get; set; }
        public string maTram { get; set; }
        public string ngayHluc { get; set; }
        public string ngaySua { get; set; }
        public string ngayTao { get; set; }
        public string nguoiSua { get; set; }
        public string nguoiTao { get; set; }
        public string tenTram { get; set; }
        public int tinhTrang { get; set; }
    }

    public class ThongTinTBiRequest
    {
        public string MA_TBI { get; set; } = string.Empty;
        public string MA_DVI_SD { get; set; } = string.Empty;
        public string LOAI_TBI { get; set; } = string.Empty;
        public string SO_TBI { get; set; } = string.Empty;
        public string MA_BDONG { get; set; } = string.Empty;
    }

    public class ThongTinTBiResponse
    {
        public JArray LST_BCS { get; set; }
        public JArray LST_TBI { get; set; }
    }

    public class TTTramRequest
    {
        public string MA_DVIQLY { get; set; }
        public string MA_TRAM { get; set; }
    }
}
