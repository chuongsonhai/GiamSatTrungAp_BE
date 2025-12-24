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
            MA_DDO_DDIEN = congvan.MaDDoDDien ?? "1";
            MA_YCAU_KNAI = congvan.MaYeuCau;
            SO_NHA = congvan.SoNha ?? "";
            DUONG_PHO = !string.IsNullOrWhiteSpace(congvan.DuongPho) ? congvan.DuongPho : congvan.DiaChiDungDien;
            TINH_TRANG = 1;
            DINH_DANH = "";
            ID_DIA_CHINH = "-1";
            CONG_SUAT = "";
            MDICH_SHOAT = congvan.DienSinhHoat ? "1" : "0";
            MDICH_CTIET = "Cấp điện trung áp";
            SO_PHA = congvan.SoPha;
            LOAI_TRAM = "";
            LOAI_TRAM = "";
            CONG_SUAT = "0";
            DTU_CTRINH = 0;
            SNGAY_YCAU = 1;
            NGUOI_SUA = congvan.NguoiDuyet ?? "trungap";
            NGUOI_TAO = congvan.NguoiLap ?? "trungap";
        }        
        public IList<TienTrinh> DV_TIEN_TRINH { get; set; }
    }
}