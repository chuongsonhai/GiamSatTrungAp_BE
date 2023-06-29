using EVN.Core.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Api.Model
{
    public class CongToModel
    {
        public CongToModel() { }
        public CongToModel(CongTo entity)
        {
            BBAN_ID = entity.BBAN_ID;
            SO_CT = entity.SO_CT;
            NAMSX_CTO = entity.NAMSX_CTO;
            MAHIEU_CTO = entity.MAHIEU_CTO;
            PHA_CTO = entity.PHA_CTO;
            LOAI_CTO = entity.LOAI_CTO;
            TSO_BIENDONG = entity.TSO_BIENDONG;
            TSO_BIENAP = entity.TSO_BIENAP;
            NGAY_KDINH = entity.NGAY_KDINH;
            TDIEN_LTRINH = entity.TDIEN_LTRINH;
            SOLAN_LTRINH = entity.SOLAN_LTRINH;
            MA_CHIHOP = entity.MA_CHIHOP;
            SO_VIENHOP = entity.SO_VIENHOP;
            MA_CHITEM = entity.MA_CHITEM;
            SO_VIENTEM = entity.SO_VIENTEM;
            SO_BIEUGIA = entity.SO_BIEUGIA;
            CHIEU_DODEM = entity.CHIEU_DODEM;
            DO_XA = entity.DO_XA;
            DONVI_HIENTHI = entity.DONVI_HIENTHI;
            HSO_MHINH = entity.HSO_MHINH;
            HSO_HTDODEM = entity.HSO_HTDODEM;
            CHI_SO = entity.CHI_SO;
            if (string.IsNullOrEmpty(entity.CHI_SO) || entity.CHI_SO == "[]")
            {
                ChiSo chieuGiao = new ChiSo();
                chieuGiao.LOAI_CHISO = "Chiều giao";
                ChiSos.Add(chieuGiao);
                //ChiSo chieuNhan = new ChiSo();
                //chieuNhan.LOAI_CHISO = "Chiều nhận";
                //ChiSos.Add(chieuNhan);
            }
            else
            {
                ChiSos = JsonConvert.DeserializeObject<List<ChiSo>>(CHI_SO);
            }
            LOAI = entity.LOAI;
        }
        public int ID { get; set; }
        public int BBAN_ID { get; set; }
        public string SO_CT { get; set; }
        public string NAMSX_CTO { get; set; }
        public string MAHIEU_CTO { get; set; }
        public string PHA_CTO { get; set; }
        public string LOAI_CTO { get; set; }
        public string TSO_BIENDONG { get; set; }
        public string TSO_BIENAP { get; set; }
        public string NGAY_KDINH { get; set; }
        public string TDIEN_LTRINH { get; set; }
        public string SOLAN_LTRINH { get; set; }
        public string MA_CHIHOP { get; set; }
        public string SO_VIENHOP { get; set; }
        public string MA_CHITEM { get; set; }
        public string SO_VIENTEM { get; set; }
        public string SO_BIEUGIA { get; set; }
        public string CHIEU_DODEM { get; set; }
        public string DO_XA { get; set; }
        public string DONVI_HIENTHI { get; set; }
        public string HSO_MHINH { get; set; }
        public string HSO_HTDODEM { get; set; }
        public string CHI_SO { get; set; }
        public List<ChiSo> ChiSos = new List<ChiSo>() { };
        public int LOAI { get; set; } = 0;

        public CongTo ToEntity(CongTo entity)
        {
            entity.SO_CT = SO_CT;
            entity.NAMSX_CTO = NAMSX_CTO;
            entity.MAHIEU_CTO = MAHIEU_CTO;
            entity.PHA_CTO = PHA_CTO;
            entity.LOAI_CTO = LOAI_CTO;
            entity.TSO_BIENDONG = TSO_BIENDONG;
            entity.TSO_BIENAP = TSO_BIENAP;
            entity.NGAY_KDINH = NGAY_KDINH;
            entity.TDIEN_LTRINH = TDIEN_LTRINH;
            entity.SOLAN_LTRINH = SOLAN_LTRINH;
            entity.MA_CHIHOP = MA_CHIHOP;
            entity.SO_VIENHOP = SO_VIENHOP;
            entity.MA_CHITEM = MA_CHITEM;
            entity.SO_VIENTEM = SO_VIENTEM;
            entity.SO_BIEUGIA = SO_BIEUGIA;
            entity.CHIEU_DODEM = CHIEU_DODEM;
            entity.DO_XA = DO_XA;
            entity.DONVI_HIENTHI = DONVI_HIENTHI;
            if (!string.IsNullOrWhiteSpace(entity.DONVI_HIENTHI))
            {
                entity.DONVI_HIENTHI = entity.DONVI_HIENTHI.ToLower() == "k" ? "kWh" : entity.DONVI_HIENTHI.ToLower() == "m" ? "MWh" : entity.DONVI_HIENTHI;
            }
            entity.HSO_MHINH = HSO_MHINH;
            entity.HSO_HTDODEM = HSO_HTDODEM;
            entity.CHI_SO = JsonConvert.SerializeObject(ChiSos);
            entity.LOAI = LOAI;
            return entity;
        }
    }
}
