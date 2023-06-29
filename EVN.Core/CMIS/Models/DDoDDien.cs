using EVN.Core.Domain;
using System;
using System.Collections.Generic;

namespace EVN.Core.CMIS
{
    public class DDoDDien : CdDoDdien
    {
        public DDoDDien(CongVanYeuCau congvan) : base()
        {
            MA_DVIQLY = congvan.MaDViQLy;
            MA_YCAU_KNAI = congvan.MaYeuCau;
            SO_NHA = congvan.SoNha ?? String.Empty;
            DUONG_PHO = !string.IsNullOrWhiteSpace(congvan.DuongPho) ? congvan.DuongPho : congvan.DiaChiDungDien; 
            ID_DIA_CHINH = "-1";
            MDICH_SHOAT = congvan.DienSinhHoat ? "1" : "0";
            MDICH_CTIET = "Cấp điện trung áp";
            SO_PHA = congvan.SoPha;
            NGUOI_SUA = congvan.NguoiDuyet;
            NGUOI_TAO = congvan.NguoiLap;
        }        
        public IList<TienTrinh> DV_TIEN_TRINH { get; set; }
    }
}