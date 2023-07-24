using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
    public class KhaoSatRequest
    {
        public KhaoSatRequest()
        {
        }
        public KhaoSatRequest(KhaoSat lKhaoSat) : base()
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
        public int ID { get; set; }
        public int CANHBAO_ID { get; set; }
        public string NOIDUNG_CAUHOI { get; set; }
        public string PHANHOI_KH { get; set; }
        public DateTime THOIGIAN_KHAOSAT { get; set; }
        public string NGUOI_KS { get; set; }
        public string KETQUA { get; set; }
        public string TINHTRANG_KT_CB { get; set; }
        public string TRANGTHAI_XOA_KHAOSAT { get; set; }
        public string DONVI_QLY { get; set; }

    }
}
