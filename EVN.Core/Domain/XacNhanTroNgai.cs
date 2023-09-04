using System;
namespace EVN.Core.Domain

{
    public class XacNhanTroNgai
    {  
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
        public virtual string SO_NGAY_TH_ND { get; set; }
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
        public virtual int TRANGTHAI { get; set; }
        public virtual int HANGMUC_KHAOSAT { get; set; }

    }
}
