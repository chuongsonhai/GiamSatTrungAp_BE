using EVN.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model.Request
{
    public class FilterLogKhaoSatRequest:BaseRequest
    {
        [JsonProperty("filter")]
        public FilterKhaoSatByCanhBaoRequest2 Filter { get; set; }
    }

    public class FilterKhaoSatByCanhBaoRequest2 : BaseRequest
    {
        public string fromdate { get; set; }
        public string todate { get; set; }
        public int TrangThaiKhaoSat { get; set; }
        public int IdCanhBao { get; set; }
        public int IdKhaoSat { get; set; }

        //trang thai canh bao
        //public int trangThai { get; set; }
        //public string donViQuanLy { get; set; }
        //public string MaYeuCau { get; set; }
    }

    public class FilterKhaoSatByCanhBaoRequest : BaseRequest
    {
        public string tuNgay { get; set; }
        public string denNgay { get; set; }
        public int TrangThaiKhaoSat { get; set; }
        public int IdCanhBao { get; set; }
        public int IdKhaoSat { get; set; }

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