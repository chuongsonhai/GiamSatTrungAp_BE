using EVN.Core.Domain;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace EVN.Core.CMIS
{
    public class CongToData
    {
        public CongToData() { }
        public CongToData(CongTo congto) : base()
        {
            SO_CT = congto.SO_CT;
            NAMSX_CTO = congto.NAMSX_CTO;
            MAHIEU_CTO = congto.MAHIEU_CTO;
            PHA_CTO = congto.PHA_CTO;
            LOAI_CTO = congto.LOAI_CTO;
            TSO_BIENDONG = congto.TSO_BIENDONG;
            TSO_BIENAP = congto.TSO_BIENAP;
            NGAY_KDINH = congto.NGAY_KDINH;
            TDIEN_LTRINH = congto.TDIEN_LTRINH;
            SOLAN_LTRINH = congto.SOLAN_LTRINH;
            MA_CHIHOP = congto.MA_CHIHOP;
            SO_VIENHOP = congto.SO_VIENHOP;
            MA_CHITEM = congto.MA_CHITEM;
            SO_VIENTEM = congto.SO_VIENTEM;
            SO_BIEUGIA = congto.SO_BIEUGIA;
            CHIEU_DODEM = congto.CHIEU_DODEM;
            DO_XA = congto.DO_XA;
            DONVI_HIENTHI = congto.DONVI_HIENTHI;
            if (!string.IsNullOrWhiteSpace(DONVI_HIENTHI))
                DONVI_HIENTHI = DONVI_HIENTHI.ToLower() == "k" ? "kWh" : DONVI_HIENTHI.ToLower() == "m" ? "MWh" : DONVI_HIENTHI;

            HSO_MHINH = congto.HSO_MHINH;
            HSO_HTDODEM = congto.HSO_HTDODEM;
            VH_CONG = congto.LOAI_CTO;
            if (!string.IsNullOrWhiteSpace(congto.CHI_SO) && congto.CHI_SO != "[]")
            {
                var chisos = JsonConvert.DeserializeObject<List<ChiSoData>>(congto.CHI_SO);
                var chiso = chisos.FirstOrDefault();
                TD = chiso.TD;
                Q = chiso.Q;
                P = chiso.P;
                BT = chiso.BT;
                CD = chiso.CD;
            }

        }
        public virtual bool CHOTCS { get; set; } = false;
        public virtual bool SDLAI { get; set; } = false;
        public virtual bool ISDOILOAI { get; set; } = false;
        public virtual string MA_CLOAI { get; set; } = "580";
        public virtual string csPgiao { get; set; } = "SG";
        public virtual string csPnhan { get; set; } = "SN";
        public virtual string SO_CT { get; set; }
        public virtual string NAMSX_CTO { get; set; }
        public virtual string MAHIEU_CTO { get; set; }
        public virtual string PHA_CTO { get; set; }
        public virtual string LOAI_CTO { get; set; }
        public virtual string TSO_BIENDONG { get; set; }
        public virtual string TSO_BIENAP { get; set; }
        public virtual string NGAY_KDINH { get; set; }
        public virtual string TDIEN_LTRINH { get; set; }
        public virtual string SOLAN_LTRINH { get; set; }
        public virtual string MA_CHIHOP { get; set; }
        public virtual string SO_VIENHOP { get; set; }
        public virtual string MA_CHITEM { get; set; }
        public virtual string SO_VIENTEM { get; set; }
        public virtual string SO_BIEUGIA { get; set; }
        public virtual string CHIEU_DODEM { get; set; }
        public virtual string DO_XA { get; set; }
        public virtual string DONVI_HIENTHI { get; set; }
        public virtual string HSO_MHINH { get; set; }
        public virtual string HSO_HTDODEM { get; set; }

        public virtual string TD { get; set; }
        public virtual string Q { get; set; }
        public virtual string BT { get; set; }
        public virtual string P { get; set; }
        public virtual string CD { get; set; }
        public virtual string VH_CONG { get; set; }
    }

    public class ChiSoData
    {
        public string LOAI_CHISO { get; set; }
        public string P { get; set; }
        public string Q { get; set; }
        public string BT { get; set; }
        public string CD { get; set; }
        public string TD { get; set; }
    }

    public class TiData
    {
        public TiData(MayBienDong entity)
        {
            SO_TBI = entity.SO_TBI;
            NAM_SX = entity.NAM_SX;
            NGAY_KDINH = entity.NGAY_KDINH;
            LOAI = entity.LOAI;
            TYSO_TI = entity.TYSO_TI;
            CHIHOP_VIEN = entity.CHIHOP_VIEN ?? string.Empty;
            TEM_KD_VIEN = entity.TEM_KD_VIEN ?? string.Empty;
        }
        public virtual string SO_TBI { get; set; }
        public virtual string NAM_SX { get; set; }
        public virtual string NGAY_KDINH { get; set; }
        public virtual string LOAI { get; set; }
        public virtual string TYSO_TI { get; set; }
        public virtual string CHIHOP_VIEN { get; set; }
        public virtual string TEM_KD_VIEN { get; set; }
    }

    public class TuData
    {
        public TuData(MayBienDienAp entity)
        {
            SO_TBI = entity.SO_TBI;
            NAM_SX = entity.NAM_SX;
            NGAY_KDINH = entity.NGAY_KDINH;
            LOAI = entity.LOAI;
            TYSO_TU = entity.TYSO_TU;
            CHIHOP_VIEN = entity.CHIHOP_VIEN ?? string.Empty;
            TEM_KD_VIEN = entity.TEM_KD_VIEN ?? string.Empty;
        }
        public virtual string SO_TBI { get; set; }
        public virtual string NAM_SX { get; set; }
        public virtual string NGAY_KDINH { get; set; }
        public virtual string LOAI { get; set; }
        public virtual string TYSO_TU { get; set; }
        public virtual string CHIHOP_VIEN { get; set; }
        public virtual string TEM_KD_VIEN { get; set; }
    }
}
