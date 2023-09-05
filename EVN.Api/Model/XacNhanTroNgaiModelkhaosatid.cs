using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class XacNhanTroNgaiModel
    {
        public XacNhanTroNgaiModel(XacNhanTroNgai lKhaoSat) : base()
        {
            ID = lKhaoSat.ID;
            MA_DVI = lKhaoSat.MA_DVI;
            MA_YCAU = lKhaoSat.MA_YCAU;
            MA_KH = lKhaoSat.MA_KH;
            TEN_KH = lKhaoSat.TEN_KH;
            DIA_CHI = lKhaoSat.DIA_CHI;
            DIEN_THOAI = lKhaoSat.DIEN_THOAI;
            MUCDICH_SD_DIEN = lKhaoSat.MUCDICH_SD_DIEN;
            NGAY_TIEPNHAN = lKhaoSat.NGAY_TIEPNHAN;
            NGAY_HOANTHANH = lKhaoSat.NGAY_HOANTHANH;
            SO_NGAY_CT = lKhaoSat.SO_NGAY_CT;
            SO_NGAY_TH_ND = lKhaoSat.SO_NGAY_TH_ND;
            TRANGTHAI_GQ = lKhaoSat.TRANGTHAI_GQ;
            TONG_CONGSUAT_CD = lKhaoSat.TONG_CONGSUAT_CD;
            DGCD_TH_CHUONGTRINH = lKhaoSat.DGCD_TH_CHUONGTRINH;
            DGCD_TH_DANGKY = lKhaoSat.DGCD_TH_DANGKY;
            DGCD_KH_PHANHOI = lKhaoSat.DGCD_KH_PHANHOI;
            CHENH_LECH = lKhaoSat.CHENH_LECH;
            DGYC_DK_DEDANG = lKhaoSat.DGYC_DK_DEDANG;
            DGYC_XACNHAN_NCHONG_KTHOI = lKhaoSat.DGYC_XACNHAN_NCHONG_KTHOI;
            DGYC_THAIDO_CNGHIEP = lKhaoSat.DGYC_THAIDO_CNGHIEP;
            DGKS_TDO_KSAT = lKhaoSat.DGKS_TDO_KSAT;
            DGKS_MINH_BACH = lKhaoSat.DGKS_MINH_BACH;
            DGKS_CHU_DAO = lKhaoSat.DGKS_CHU_DAO;
            DGNT_THUAN_TIEN = lKhaoSat.DGNT_THUAN_TIEN;
            DGNT_MINH_BACH = lKhaoSat.DGNT_MINH_BACH;
            DGNT_CHU_DAO = lKhaoSat.DGNT_CHU_DAO;
            KSAT_CHI_PHI = lKhaoSat.KSAT_CHI_PHI;
            DGHL_CAPDIEN = lKhaoSat.DGHL_CAPDIEN;
            TRANGTHAI_GOI = lKhaoSat.TRANGTHAI_GOI;
            NGAY = lKhaoSat.NGAY;
            NGUOI_KSAT = lKhaoSat.NGUOI_KSAT;
            Y_KIEN_KH = lKhaoSat.Y_KIEN_KH;
            NOIDUNG = lKhaoSat.NOIDUNG;
            PHAN_HOI = lKhaoSat.PHAN_HOI;
            GHI_CHU = lKhaoSat.GHI_CHU;
            TRANGTHAI = lKhaoSat.TRANGTHAI;

        }
        public virtual int ID { get; set; }
        public virtual string MA_DVI { get; set; }
        public virtual string MA_YCAU { get; set; }
        public virtual string MA_KH { get; set; }
        public virtual string TEN_KH { get; set; }
        public virtual string DIA_CHI { get; set; }
        public virtual string DIEN_THOAI { get; set; }
        public virtual string MUCDICH_SD_DIEN { get; set; }
        public virtual DateTime NGAY_TIEPNHAN { get; set; } = DateTime.Now;
        public virtual DateTime NGAY_HOANTHANH { get; set; } = DateTime.Now;
        public virtual string SO_NGAY_CT { get; set; }
        public virtual int SO_NGAY_TH_ND { get; set; }
        public virtual int TRANGTHAI_GQ { get; set; }
        public virtual int TONG_CONGSUAT_CD { get; set; }
        public virtual double DGCD_TH_CHUONGTRINH { get; set; }
        public virtual double DGCD_TH_DANGKY { get; set; }
        public virtual int DGCD_KH_PHANHOI { get; set; }
        public virtual double CHENH_LECH { get; set; }
        public virtual int DGYC_DK_DEDANG { get; set; }
        public virtual int DGYC_XACNHAN_NCHONG_KTHOI { get; set; }
        public virtual int DGYC_THAIDO_CNGHIEP { get; set; }
        public virtual int DGKS_TDO_KSAT { get; set; }
        public virtual int DGKS_MINH_BACH { get; set; }
        public virtual int DGKS_CHU_DAO { get; set; }
        public virtual int DGNT_THUAN_TIEN { get; set; }
        public virtual int DGNT_MINH_BACH { get; set; }
        public virtual int DGNT_CHU_DAO { get; set; }
        public virtual int KSAT_CHI_PHI { get; set; }
        public virtual int DGHL_CAPDIEN { get; set; }
        public virtual int TRANGTHAI_GOI { get; set; }
        public virtual DateTime NGAY { get; set; } = DateTime.Now;
        public virtual string NGUOI_KSAT { get; set; }
        public virtual string Y_KIEN_KH { get; set; }
        public virtual string NOIDUNG { get; set; }
        public virtual string PHAN_HOI { get; set; }
        public virtual string GHI_CHU { get; set; }
        public virtual int CANHBAO_ID { get; set; }
        public virtual int TRANGTHAI { get; set; }
    }
   
}
