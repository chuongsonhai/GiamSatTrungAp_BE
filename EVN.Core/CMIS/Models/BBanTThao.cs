using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.CMIS
{
    public class BBTreoThaoRequest
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public BBanTThao BBanTThao { get; set; }
        public TTinYCau TTinYCau { get; set; }
    }

    public class BBanTThao
    {
        public BBanTThao(BienBanTT bienban)
        {
            var nviensrv = IoC.Resolve<INhanVienService>();

            TEN_CTY = bienban.TEN_CTY;
            TEN_DLUC = bienban.TEN_DLUC;
            SO_BB = bienban.SO_BB;
            LY_DO = bienban.LY_DO;
            MA_LDO = bienban.MA_LDO;
            MO_TA = bienban.MO_TA;
            TEN_KHACHHANG = bienban.TEN_KHACHHANG;
            SDT_KHACHHANG = bienban.SDT_KHACHHANG;
            NGUOI_DDIEN = bienban.NGUOI_DDIEN;
            DIA_DIEM = bienban.DIA_DIEM;
            MA_DDO = bienban.MA_DDO;
            MA_TRAM = bienban.MA_TRAM;
            MA_GCS = bienban.MA_GCS;
            VTRI_LDAT = bienban.VTRI_LDAT;
            
            SO_COT = bienban.SO_COT;
            SO_HOP = bienban.SO_HOP;

            MA_NVIENTTHAO = string.IsNullOrEmpty(bienban.NVIEN_TTHAO) ? "" : bienban.NVIEN_TTHAO.Split('-')[0];
            MA_NVIENTTHAO2 = string.IsNullOrEmpty(bienban.NVIEN_TTHAO2) ? "" : bienban.NVIEN_TTHAO2.Split('-')[0];
            MA_NVIENTTHAO3 = string.IsNullOrEmpty(bienban.NVIEN_TTHAO3) ? "" : bienban.NVIEN_TTHAO3.Split('-')[0];
            MA_NVIENNPHONG = string.IsNullOrEmpty(bienban.NVIEN_NPHONG) ? "" : bienban.NVIEN_NPHONG.Split('-')[0];

            if (!string.IsNullOrWhiteSpace(MA_NVIENTTHAO))
            {
                var nvientthao = nviensrv.GetbyCode(bienban.MA_DVIQLY, MA_NVIENTTHAO);
                NVIEN_TTHAO = nvientthao != null ? $"{MA_NVIENTTHAO}-{nvientthao.TEN_NVIEN}" : "";
            }
            
            if (!string.IsNullOrWhiteSpace(MA_NVIENTTHAO2))
            {
                var nvientthao2 = nviensrv.GetbyCode(bienban.MA_DVIQLY, MA_NVIENTTHAO2);
                NVIEN_TTHAO2 = nvientthao2 != null ? $"{MA_NVIENTTHAO2}-{nvientthao2.TEN_NVIEN}" : "";
            }

            if (!string.IsNullOrWhiteSpace(MA_NVIENTTHAO3))
            {
                var nvientthao3 = nviensrv.GetbyCode(bienban.MA_DVIQLY, MA_NVIENTTHAO3);
                NVIEN_TTHAO3 = nvientthao3 != null ? $"{MA_NVIENTTHAO3}-{nvientthao3.TEN_NVIEN}" : "";
                
            }
            
            if (!string.IsNullOrWhiteSpace(MA_NVIENNPHONG))
            {
                var nviennphong = nviensrv.GetbyCode(bienban.MA_DVIQLY, MA_NVIENNPHONG);
                NVIEN_NPHONG = nviennphong != null ? $"{MA_NVIENNPHONG}-{nviennphong.TEN_NVIEN}" : "";
            }
            
            NHANVIEN_BOPHAN nvtt = new NHANVIEN_BOPHAN();
            nvtt.MA_NVIEN = MA_NVIENTTHAO;
            nvtt.MA_DTUONG_KY = "NVTT";
            NHANVIEN_BOPHAN.Add(nvtt);
            NHANVIEN_BOPHAN nvtt2 = new NHANVIEN_BOPHAN();
            nvtt2.MA_NVIEN = MA_NVIENTTHAO2;
            nvtt2.MA_DTUONG_KY = "NVTT2";
            NHANVIEN_BOPHAN.Add(nvtt2);
            NHANVIEN_BOPHAN nvtt3 = new NHANVIEN_BOPHAN();
            nvtt3.MA_NVIEN = MA_NVIENTTHAO3;
            nvtt3.MA_DTUONG_KY = "NVTT3";
            NHANVIEN_BOPHAN.Add(nvtt3);
            NHANVIEN_BOPHAN nvnp = new NHANVIEN_BOPHAN();
            nvnp.MA_NVIEN = MA_NVIENNPHONG;
            nvnp.MA_DTUONG_KY = "NVNP";
            NHANVIEN_BOPHAN.Add(nvnp);

            MA_NVIENTTHAO = NVIEN_TTHAO;
            MA_NVIENTTHAO2 = NVIEN_TTHAO2;
            MA_NVIENTTHAO3 = NVIEN_TTHAO3;
            MA_NVIENNPHONG = NVIEN_NPHONG;
        }
        public string NAM_HT { get; set; }
        public string NGAY_HT { get; set; }
        public string THANG_HT { get; set; }
        public string MAU_PHIEU { get; set; } = "BBAN_TTHAO";
        public string TEN_TINH { get; set; } = "Hà Nội";
        public string SO_COT { get; set; }
        public string SO_HOP { get; set; }

        public List<NHANVIEN_BOPHAN> NHANVIEN_BOPHAN { get; set; } = new List<NHANVIEN_BOPHAN>();

        public string MA_LDO { get; set; }

        public string TEN_CTY { get; set; }
        public string TEN_DLUC { get; set; }
        public string SO_BB { get; set; }
        public string LY_DO { get; set; } = "Gắn mới";
        public string MO_TA { get; set; }
        public string TEN_KHACHHANG { get; set; }
        public string SDT_KHACHHANG { get; set; }
        public string NGUOI_DDIEN { get; set; }
        public string DIA_DIEM { get; set; }
        public string MA_DDO { get; set; }
        public string MA_TRAM { get; set; }
        public string MA_GCS { get; set; }
        public string VTRI_LDAT { get; set; }
        public string MA_NVIENTTHAO { get; set; }
        public string MA_NVIENTTHAO2 { get; set; }
        public string MA_NVIENTTHAO3 { get; set; }
        public string MA_NVIENNPHONG { get; set; }
        public string NVIEN_TTHAO { get; set; }
        public string NVIEN_TTHAO2 { get; set; }
        public string NVIEN_TTHAO3 { get; set; }
        public string NVIEN_NPHONG { get; set; }
        public TreoThaoData THAO { get; set; }
        public TreoThaoData TREO { get; set; }

        public void Init(IList<CongTo> congTos, IList<MayBienDong> mayBienDongs, IList<MayBienDienAp> mayBienDienAps)
        {
            DateTime today = DateTime.Today;
            var congtoThao = congTos.FirstOrDefault(p => p.LOAI == 0);
            var listTIThao = mayBienDongs.Where(p => p.TI_THAO).ToList();
            var listTUThao = mayBienDienAps.Where(p => p.TU_THAO).ToList();

            NAM_HT = today.Year.ToString();
            THANG_HT = today.Month.ToString();
            NGAY_HT = today.Day.ToString();

            THAO = new TreoThaoData();
            TREO = new TreoThaoData();
            THAO.CONG_TO = congtoThao != null ? new CongToData(congtoThao) : new CongToData();
            THAO.TI = new List<TiData>();
            foreach (var ti in listTIThao)
            {
                THAO.TI.Add(new TiData(ti));
            }
            THAO.TU = new List<TuData>();
            foreach (var ti in listTUThao)
            {
                THAO.TU.Add(new TuData(ti));
            }

            var congtoTreo = congTos.FirstOrDefault(p => p.LOAI == 1);
            var listTITreo = mayBienDongs.Where(p => !p.TI_THAO).ToList();
            var listTUTreo = mayBienDienAps.Where(p => !p.TU_THAO).ToList();

            TREO.CONG_TO = congtoTreo != null ? new CongToData(congtoTreo) : new CongToData();
            TREO.HSNDD_TREO = congtoTreo != null ? congtoTreo.HSO_HTDODEM : string.Empty;
            TREO.TI = new List<TiData>();
            foreach (var ti in listTITreo)
            {
                var tidata = new TiData(ti);                
                tidata.CHIHOP_VIEN = !string.IsNullOrWhiteSpace(tidata.CHIHOP_VIEN) ? tidata.CHIHOP_VIEN : congtoTreo.MA_CHIHOP;
                TREO.TI.Add(tidata);
            }
            TREO.TU = new List<TuData>();
            foreach (var ti in listTUTreo)
            {
                TREO.TU.Add(new TuData(ti));
            }
        }
    }

    public class TTinYCau
    {
        public TTinYCau(DvTienTrinh tienTrinh)
        {
            MA_BPHAN_GIAO = tienTrinh.MA_BPHAN_GIAO;
            MA_CNANG = tienTrinh.MA_CNANG;
            MA_CVIEC = tienTrinh.MA_CVIEC;
            MA_DDO_DDIEN = tienTrinh.MA_DDO_DDIEN;
            MA_DVIQLY = tienTrinh.MA_DVIQLY;
            MA_NVIEN_GIAO = tienTrinh.MA_NVIEN_GIAO;
            MA_TNGAI = tienTrinh.MA_TNGAI;
            MA_YCAU_KNAI = tienTrinh.MA_YCAU_KNAI;
            NDUNG_XLY = tienTrinh.NDUNG_XLY;
            NGAY_BDAU = tienTrinh.NGAY_BDAU.ToString("dd/MM/yyyy");
            NGAY_HEN = tienTrinh.NGAY_HEN.ToString("dd/MM/yyyy");
            NGAY_KTHUC = tienTrinh.NGAY_KTHUC.Value.ToString("dd/MM/yyyy");
            USER = tienTrinh.NGUOI_TAO;
        }
        public virtual string KQ_ID_BUOC { get; set; } = "-1";

        public virtual string MA_BPHAN_GIAO { get; set; }
        public virtual string MA_NVIEN_GIAO { get; set; }

        public virtual string MA_CNANG { get; set; } = "WEB_TBAC_D";
        public virtual string MA_CVIEC { get; set; }
        public virtual string MA_DDO_DDIEN { get; set; } //Mã KH
        public virtual string MA_DVIQLY { get; set; }

        public virtual string MA_TNGAI { get; set; } = string.Empty;
        public virtual string MA_YCAU_KNAI { get; set; }
        public virtual string MA_LOAI_YCAU { get; set; } = "TABC_D";
        public virtual string NDUNG_XLY { get; set; } = string.Empty;

        public virtual string NGAY_BDAU { get; set; }
        public virtual string NGAY_HEN { get; set; }
        public virtual string NGAY_KTHUC { get; set; }

        public virtual string NGUYEN_NHAN { get; set; } = string.Empty;

        public virtual string USER { get; set; } = string.Empty;
    }

    public class NHANVIEN_BOPHAN
    {

        public string MA_NVIEN { get; set; }
        public string MA_DTUONG_KY { get; set; }
    }
}
