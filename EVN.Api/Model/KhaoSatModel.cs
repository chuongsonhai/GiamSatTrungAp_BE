using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class KhaoSatModel
    {
        public KhaoSatModel(KhaoSat lKhaoSat) : base()
        {
            ID = lKhaoSat.ID;
            CANHBAO_ID = lKhaoSat.CANHBAO_ID;
            NOIDUNG_CAUHOI = lKhaoSat.NOIDUNG_CAUHOI;
            PHANHOI_KH = lKhaoSat.PHANHOI_KH;
            THOIGIAN_KHAOSAT = lKhaoSat.THOIGIAN_KHAOSAT;
            NGUOI_KS = lKhaoSat.NGUOI_KS;
            KETQUA = lKhaoSat.KETQUA;
            TINHTRANG_KT_CB = lKhaoSat.TINHTRANG_KT_CB;
            TRANGTHAI_XOA_KHAOSAT = lKhaoSat.TRANGTHAI_XOA_KHAOSAT;
            DONVI_QLY = lKhaoSat.DONVI_QLY;

        }
        public virtual int ID { get; set; }
        public virtual int CANHBAO_ID { get; set; }
        public virtual string NOIDUNG_CAUHOI { get; set; }
        public virtual string PHANHOI_KH { get; set; }
        public virtual DateTime THOIGIAN_KHAOSAT { get; set; }
        public virtual string NGUOI_KS { get; set; }
        public virtual string KETQUA { get; set; }
        public virtual string TINHTRANG_KT_CB { get; set; }
        public virtual string TRANGTHAI_XOA_KHAOSAT { get; set; }
        public virtual string DONVI_QLY { get; set; }
    }
   
}
