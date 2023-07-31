using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
    public class PhanhoiTraodoiRequest
    {
        public PhanhoiTraodoiRequest()
        {
        }
        public PhanhoiTraodoiRequest(PhanhoiTraodoi lPhanhoiTraodoi) : base()
        {
            ID = lPhanhoiTraodoi.ID;
            CANHBAO_ID = lPhanhoiTraodoi.CANHBAO_ID;
            NOIDUNG_PHANHOI = lPhanhoiTraodoi.NOIDUNG_PHANHOI;
            NGUOI_GUI = lPhanhoiTraodoi.NGUOI_GUI;
            DONVI_QLY = lPhanhoiTraodoi.DONVI_QLY;
            THOIGIAN_GUI = lPhanhoiTraodoi.THOIGIAN_GUI;
            TRANGTHAI_XOA = lPhanhoiTraodoi.TRANGTHAI_XOA;
        }
        public int ID { get; set; }
        public int CANHBAO_ID { get; set; }
        public string NOIDUNG_PHANHOI { get; set; }
        public string NGUOI_GUI { get; set; }
        public string DONVI_QLY { get; set; }
        public DateTime THOIGIAN_GUI { get; set; } = DateTime.Now;
        public int TRANGTHAI_XOA { get; set; }
        public string FILE_DINHKEM { get; set; }

    }
}
