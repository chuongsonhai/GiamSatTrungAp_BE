using EVN.Core.Domain;
using System;
using System.Collections.Generic;

namespace EVN.Core.CMIS
{

    public class ThongTinUQResponse
    {
        public string strError { get; set; }
        public IList<ThongTinUQ> LST_OBJ { get; set; } = new List<ThongTinUQ>();
    }    
    public class ThongTinUQ
    {
        public ThongTinUQ() { }

        public string chucVu { get; set; }
        public string cvuUquyen { get; set; }
        public string dchiDviuq { get; set; }
        public string idKey { get; set; }
        public string idUquyen { get; set; }
        public string maDviqly { get; set; }
        public string ngayHhluc { get; set; }
        public string ngayHluc { get; set; }
        public string ngaySua { get; set; }
        public string ngayTao { get; set; }
        public string ngayUquyen { get; set; }
        public string soUquyen { get; set; }
        public string tenUquyen { get; set; }
        public string tnguoiUquyen { get; set; }
    }

    public class ThongTinNhomGia
    {
        public string DON_GIA { get; set; }
        public string ID_GIANHOMNN { get; set; }
        public string KHOANG_DA { get; set; }
        public string LOAI_TIEN { get; set; }
        public string MA_NGIA { get; set; }
        public string MA_NHOMNN { get; set; }
        public string MOTA_GIA { get; set; }
        public string NGAY_ADUNG { get; set; }
        public string SOTHUTU { get; set; }
        public string THOIGIAN_BDIEN { get; set; }
        public string MA_CAPDA { get; set; }
        public string TEN_NHOMNN { get; set; }
    }
}
