using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
    public class XacNhanTroNgaiRequest
    {
        public XacNhanTroNgaiRequest()
        {
        }
        public XacNhanTroNgaiRequest(XacNhanTroNgai lKhaoSat) : base()
        {
            ID = lKhaoSat.ID;
            //NOIDUNG_CAUHOI = lKhaoSat.NOIDUNG_CAUHOI;
            //PHANHOI_KH = lKhaoSat.PHANHOI_KH;
            //THOIGIAN_KHAOSAT = lKhaoSat.THOIGIAN_KHAOSAT;
            //NGUOI_KS = lKhaoSat.NGUOI_KS;
            //KETQUA = lKhaoSat.KETQUA;
            //TINHTRANG_KT_CB = lKhaoSat.TINHTRANG_KT_CB;
            //TRANGTHAI = lKhaoSat.TRANGTHAI;
            //DONVI_QLY = lKhaoSat.DONVI_QLY;

        }

     
        public int ID { get; set; }
        public int CANHBAO_ID { get; set; }
        public string NOIDUNG_CAUHOI { get; set; }
        public string PHANHOI_KH { get; set; }
        public DateTime THOIGIAN_KHAOSAT { get; set; }
        public string NGUOI_KS { get; set; }
        public string KETQUA { get; set; }
        public string TINHTRANG_KT_CB { get; set; }
        public int TRANGTHAI { get; set; }
        public string DONVI_QLY { get; set; }

    }
    public class XacNhanTroNgaikhaosatadd
    {
        public XacNhanTroNgaikhaosatadd()
        {
        }
        public XacNhanTroNgaikhaosatadd(XacNhanTroNgai lKhaoSat) : base()
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
        public int ID { get; set; }
        public string MA_DVI { get; set; }
        public string MA_YCAU { get; set; }
        public string MA_KH { get; set; }
        public string TEN_KH { get; set; }
        public string DIA_CHI { get; set; }
        public string DIEN_THOAI { get; set; }
        public string MUCDICH_SD_DIEN { get; set; }
        public DateTime NGAY_TIEPNHAN { get; set; } = DateTime.Now;
        public DateTime NGAY_HOANTHANH { get; set; } = DateTime.Now;
        public string SO_NGAY_CT { get; set; }
        public string SO_NGAY_TH_ND { get; set; }
        public int? TRANGTHAI_GQ { get; set; }
        public int? TONG_CONGSUAT_CD { get; set; }
        public int? DGCD_TH_CHUONGTRINH { get; set; }
        public int? DGCD_TH_DANGKY { get; set; }
        public int? DGCD_KH_PHANHOI { get; set; }
        public int? DGYC_DK_DEDANG { get; set; }
        public int? DGYC_XACNHAN_NCHONG_KTHOI { get; set; }
        public int? DGYC_THAIDO_CNGHIEP { get; set; }
        public int? DGKS_TDO_KSAT { get; set; }
        public int? DGKS_MINH_BACH { get; set; }
        public int? DGKS_CHU_DAO { get; set; }
        public int? DGNT_THUAN_TIEN { get; set; }
        public int? DGNT_MINH_BACH { get; set; }
        public int? DGNT_CHU_DAO { get; set; }
        public int? KSAT_CHI_PHI { get; set; }
        public int? DGHL_CAPDIEN { get; set; }
        public int? TRANGTHAI_GOI { get; set; }

        public DateTime NGAY { get; set; } = DateTime.Now;
        public string NGUOI_KSAT { get; set; }
        public string Y_KIEN_KH { get; set; }
        public string NOIDUNG { get; set; }
        public string PHAN_HOI { get; set; }
        public string GHI_CHU { get; set; }
        public int? CANHBAO_ID { get; set; }
        public string PHANHOI_KH { get; set; }
        public string PHANHOI_DV { get; set; }
        public string KETQUA { get; set; }
        public string NGUOI_KS { get; set; }
        public string MA_YC { get; set; }
        public int? TRANGTHAI { get; set; }
        public int? HANGMUC_KHAOSAT { get; set; }
    }

    public class XacNhanTroNgakhaosatid
    {
        public XacNhanTroNgakhaosatid()
        {
        }
        public XacNhanTroNgakhaosatid(XacNhanTroNgai lKhaoSat) : base()
        {
            idKhaoSat = lKhaoSat.ID;
            //noiDungKhaoSat = lKhaoSat.NOIDUNG_CAUHOI;
            //khachHangPhanHoi = lKhaoSat.PHANHOI_KH;
            //ketQuaKhaoSat = lKhaoSat.KETQUA;
            //nguoiKhaoSat = lKhaoSat.NGUOI_KS;
            trangThaiKhaoSat = lKhaoSat.TRANGTHAI;

        }


        public int idKhaoSat { get; set; }
        public string noiDungKhaoSat { get; set; }
        public string khachHangPhanHoi { get; set; }
        public string nguoiKhaoSat { get; set; }
        public string ketQuaKhaoSat { get; set; }
        public int? trangThaiKhaoSat { get; set; }


    }



}
