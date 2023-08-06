using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
    public class PhanhoiTraodoiRequestid
    {
        public PhanhoiTraodoiRequestid()
        {
        }
        public PhanhoiTraodoiRequestid(PhanhoiTraodoi lPhanhoiTraodoi) : base()
        {
            idPhanHoi = lPhanhoiTraodoi.ID;
            noiDungPhanHoi = lPhanhoiTraodoi.NOIDUNG_PHANHOI;
            nguoiGui = lPhanhoiTraodoi.NGUOI_GUI;
            thoiGianGui = lPhanhoiTraodoi.THOIGIAN_GUI;
        }
        public int idPhanHoi { get; set; }
        public string noiDungPhanHoi { get; set; }
        public string nguoiGui { get; set; }
        public DateTime thoiGianGui { get; set; } = DateTime.Now;


    }
}
