using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class KetThucCanhbaoModel
    {
        public KetThucCanhbaoModel()
        {
        }
        public KetThucCanhbaoModel(CanhBao CanhBao) : base()
        {
            ID = CanhBao.ID;
            LOAI_CANHBAO_ID = CanhBao.LOAI_CANHBAO_ID;
            NOIDUNG = CanhBao.NOIDUNG;
            THOIGIANGUI = CanhBao.THOIGIANGUI;
            DONVI_DIENLUC = CanhBao.DONVI_DIENLUC;
            TRANGTHAI_CANHBAO = CanhBao.TRANGTHAI_CANHBAO;
            LOAI_SOLANGUI = CanhBao.LOAI_SOLANGUI;
            MA_YC = CanhBao.MA_YC;
            NGUYENHHAN_CANHBAO = CanhBao.NGUYENHHAN_CANHBAO;
        }
        public int ID { get; set; }
        public int LOAI_CANHBAO_ID { get; set; }
        //  public virtual int TRANGTHAI_GUI { get; set; }
        public DateTime THOIGIANGUI { get; set; } = DateTime.Now;
        public string NOIDUNG { get; set; }
        public int LOAI_SOLANGUI { get; set; }
        public string MA_YC { get; set; }
        public int TRANGTHAI_CANHBAO { get; set; }
        public string DONVI_DIENLUC { get; set; }
        public int NGUYENHHAN_CANHBAO { get; set; }

    }
}
