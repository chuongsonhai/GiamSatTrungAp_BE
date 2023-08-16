using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model.Request
{
    public class FilterKhaoSatByCanhBaoRequest
    {
        public string tuNgay { get; set; }
        public string denNgay { get; set; }
        public int TrangThaiKhaoSat { get; set; }
        public int IdCanhBao { get; set; }

        //trang thai canh bao
        //public int trangThai { get; set; }
        //public string donViQuanLy { get; set; }
        //public string MaYeuCau { get; set; }
    }
    public class FilterKhaoSatByCanhBaologRequest
    {
        public string tuNgay { get; set; }
        public string denNgay { get; set; }
        //trang thai canh bao
        public int trangThai { get; set; }
        public string donViQuanLy { get; set; }
        public string MaYeuCau { get; set; }
    }
}