using EVN.Core;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using System;
using System.Collections.Generic;

namespace EVN.Api.Model
{
    public class BienBanTTModel
    {
        public BienBanTTModel() { }
        public BienBanTTModel(BienBanTT entity)
        {
            ID = entity.ID; ;
            MA_DVIQLY = entity.MA_DVIQLY; ;
            MA_YCAU_KNAI = entity.MA_YCAU_KNAI;
            TEN_CTY = entity.TEN_CTY;
            TEN_DLUC = entity.TEN_DLUC;
            SO_BB = entity.SO_BB;
            LY_DO = entity.LY_DO;
            MO_TA = entity.MO_TA;
            TEN_KHACHHANG = entity.TEN_KHACHHANG;
            SDT_KHACHHANG = entity.SDT_KHACHHANG;
            NGUOI_DDIEN = entity.NGUOI_DDIEN;
            DIA_DIEM = entity.DIA_DIEM;
            MA_DDO = entity.MA_DDO;
            MA_TRAM = entity.MA_TRAM;
            MA_GCS = entity.MA_GCS;
            VTRI_LDAT = entity.VTRI_LDAT;
            NVIEN_TTHAO = entity.NVIEN_TTHAO;
            NVIEN_TTHAO2 = entity.NVIEN_TTHAO2;
            NVIEN_TTHAO3 = entity.NVIEN_TTHAO3;
            NVIEN_NPHONG = entity.NVIEN_NPHONG;
            NGUOI_TAO = entity.NGUOI_TAO;
            NGAY_TAO = entity.NGAY_TAO.ToString("dd/MM/yyyy");
            TRANG_THAI = entity.TRANG_THAI;
            SO_COT = entity.SO_COT;
            SO_HOP= entity.SO_HOP;
            Data = entity.Data;
            NoiDungXuLy = entity.NoiDungXuLy;
            KyNVTT = entity.KyNVTT;
            KyNVNP = entity.KyNVNP;

            ICongToService congToService = IoC.Resolve<ICongToService>();
            IMayBienDongService mayBienDongService = IoC.Resolve<IMayBienDongService>();
            IMayBienDienApService mayBienDienApService = IoC.Resolve<IMayBienDienApService>();
            if (ID > 0)
            {
                var congToTreo = congToService.GetByBBTTID(ID, 1);
                if (congToTreo != null)
                {
                    ChiTietThietBiTreo.IsCongTo = true;
                    ChiTietThietBiTreo.CongTo = new CongToModel(congToTreo);
                }
                else
                {
                    congToTreo = new CongTo();
                    ChiTietThietBiTreo.IsCongTo = true;
                    ChiTietThietBiTreo.CongTo = new CongToModel(congToTreo);
                }
                var lstMBDTreo = mayBienDongService.GetByBBTTID(ID, false);
                if (lstMBDTreo.Count > 0)
                {
                    ChiTietThietBiTreo.IsMBD = true;
                }
                foreach (var mbDTreo in lstMBDTreo)
                {
                    MayBienDongModel mayBienDongModel = new MayBienDongModel(mbDTreo);
                    mayBienDongModel.CHIHOP_VIEN = !string.IsNullOrWhiteSpace(mayBienDongModel.CHIHOP_VIEN) ? mayBienDongModel.CHIHOP_VIEN : congToTreo.MA_CHIHOP;
                    ChiTietThietBiTreo.MayBienDongs.Add(mayBienDongModel);
                }
                var lstMBDATreo = mayBienDienApService.GetByBBTTID(ID, false);
                if (lstMBDATreo.Count > 0)
                {
                    ChiTietThietBiTreo.IsMBDA = true;
                }
                foreach (var mbDATreo in lstMBDATreo)
                {
                    MayBienDienApModel mayBienDienApModel = new MayBienDienApModel(mbDATreo);
                    ChiTietThietBiTreo.MayBienDienAps.Add(mayBienDienApModel);
                }

                var congToThao = congToService.GetByBBTTID(ID, 0);
                if (congToThao != null)
                {
                    ChiTietThietBiThao.CongTo = new CongToModel(congToThao);
                }
                var lstMBDThao = mayBienDongService.GetByBBTTID(ID, true);
                foreach (var mbDThao in lstMBDThao)
                {
                    MayBienDongModel mayBienDongModel = new MayBienDongModel(mbDThao);
                    ChiTietThietBiThao.MayBienDongs.Add(mayBienDongModel);
                }
                var lstMBDAThao = mayBienDienApService.GetByBBTTID(ID, true);
                foreach (var mbDAThao in lstMBDAThao)
                {
                    MayBienDienApModel mayBienDienApModel = new MayBienDienApModel(mbDAThao);
                    ChiTietThietBiThao.MayBienDienAps.Add(mayBienDienApModel);
                }
            }
            else
            {
                var congToTreo = new CongTo();
                ChiTietThietBiTreo.CongTo = new CongToModel(congToTreo);
                var congToThao = new CongTo();
                ChiTietThietBiThao.CongTo = new CongToModel(congToThao);

            }
        }
        public int ID { get; set; }
        public string MA_DVIQLY { get; set; }
        public string MA_YCAU_KNAI { get; set; }
        public string TEN_CTY { get; set; }
        public string TEN_DLUC { get; set; }
        public string SO_BB { get; set; }
        public string LY_DO { get; set; }
        public string MA_LDO { get; set; }
        public string MO_TA { get; set; }
        public string TEN_KHACHHANG { get; set; }
        public string SDT_KHACHHANG { get; set; }
        public string NGUOI_DDIEN { get; set; }
        public string DIA_DIEM { get; set; }
        public string MA_DDO { get; set; }
        public string MA_TRAM { get; set; }
        public string MA_GCS { get; set; }
        public string VTRI_LDAT { get; set; }
        public string NVIEN_TTHAO { get; set; }
        public string NVIEN_TTHAO2 { get; set; }
        public string NVIEN_TTHAO3 { get; set; }
        public string NVIEN_NPHONG { get; set; }
        public string NGUOI_TAO { get; set; }
        public string NGAY_TAO { get; set; }
        public int TRANG_THAI { get; set; } = 0;
        public string SO_COT { get; set; }
        public string SO_HOP { get; set; }
        public string Data { get; set; }
        public string NoiDungXuLy { get; set; }

        public bool KyNVTT { get; set; } = false;
        public bool KyNVNP { get; set; } = false;

        public BienBanTT ToEntity(BienBanTT entity)
        {
            entity.MA_DVIQLY = MA_DVIQLY; ;
            entity.MA_YCAU_KNAI = MA_YCAU_KNAI;
            entity.TEN_CTY = TEN_CTY;
            entity.TEN_DLUC = TEN_DLUC;
            entity.SO_BB = SO_BB;
            entity.LY_DO = LY_DO;
            entity.MA_LDO = MA_LDO;
            entity.MO_TA = MO_TA;
            entity.TEN_KHACHHANG = TEN_KHACHHANG;
            entity.SDT_KHACHHANG = SDT_KHACHHANG;
            entity.NGUOI_DDIEN = NGUOI_DDIEN;
            entity.DIA_DIEM = DIA_DIEM;
            entity.MA_DDO = MA_DDO;
            entity.MA_TRAM = MA_TRAM;
            entity.MA_GCS = MA_GCS;
            entity.SO_COT = SO_COT;
            entity.SO_HOP = SO_HOP;
            int vtrildat = 0;
            entity.VTRI_LDAT = VTRI_LDAT;
            if (int.TryParse(VTRI_LDAT, out vtrildat))
            {
                string vtldat = vtrildat == 0 ? "2" : vtrildat == 2 ? "0" : "1";
                entity.VTRI_LDAT = vtldat;
                if (vtrildat != 2)
                    entity.SO_COT = entity.SO_HOP = String.Empty;
            }

            entity.NVIEN_TTHAO = NVIEN_TTHAO;
            entity.NVIEN_TTHAO2 = NVIEN_TTHAO2;
            entity.NVIEN_TTHAO3 = NVIEN_TTHAO3;
            entity.NVIEN_NPHONG = NVIEN_NPHONG;
            entity.NGUOI_TAO = NGUOI_TAO;
            if (!string.IsNullOrWhiteSpace(NGAY_TAO))
                entity.NGAY_TAO = DateTime.ParseExact(NGAY_TAO, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            entity.TRANG_THAI = TRANG_THAI;
            return entity;
        }
        public ChiTietThietBiTreoModel ChiTietThietBiTreo { get; set; } = new ChiTietThietBiTreoModel();
        public ChiTietThietBiThaoModel ChiTietThietBiThao { get; set; } = new ChiTietThietBiThaoModel();
    }

    public class ChiTietThietBiTreoModel
    {
        public CongToModel CongTo { get; set; } = new CongToModel();
        public bool IsCongTo { get; set; }
        public bool IsMBD { get; set; }
        public bool IsMBDA { get; set; }
        public IList<MayBienDongModel> MayBienDongs { get; set; } = new List<MayBienDongModel>();
        public IList<MayBienDienApModel> MayBienDienAps { get; set; } = new List<MayBienDienApModel>();
    }
    public class ChiTietThietBiThaoModel
    {
        public CongToModel CongTo { get; set; } = new CongToModel();
        public IList<MayBienDongModel> MayBienDongs { get; set; } = new List<MayBienDongModel>();
        public IList<MayBienDienApModel> MayBienDienAps { get; set; } = new List<MayBienDienApModel>();
    }

    public class ChiSo
    {
        public string LOAI_CHISO { get; set; }
        public string P { get; set; }
        public string Q { get; set; }
        public string BT { get; set; }
        public string CD { get; set; }
        public string TD { get; set; }
    }

    public class SignBienBanTT
    {
        public int id { get; set; }
        public bool SignTT { get; set; }        
    }
}