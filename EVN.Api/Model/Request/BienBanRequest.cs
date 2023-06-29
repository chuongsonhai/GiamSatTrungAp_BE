using EVN.Core;
using Newtonsoft.Json;
using System;

namespace EVN.Api.Model.Request
{
    public class BienBanRequest : BaseRequest
    {
        [JsonProperty("Filter")]
        public BienBanFilter Filter { get; set; }
    }

    public class BienBanFilter
    {
        public BienBanFilter()
        {
            var today = DateTime.Today;
            fromdate = new DateTime(today.Year, today.Month, 1).ToString("dd/MM/yyyy");
            todate = today.ToString("dd/MM/yyyy");
        }
        public string maDViQly { get; set; }
        public string maYCau { get; set; }
        public string keyword { get; set; } = string.Empty;
        public string fromdate { get; set; }
        public string todate { get; set; }
        public int status { get; set; } = 0;
        public bool isHoanTat { get; set; } = true;
        public int thang { get; set; } = 0;
        public int nam { get; set; } = 0;
    }


    public class ReportRequest : BaseRequest
    {
        [JsonProperty("Filter")]
        public ReportFilter Filter { get; set; }
    }

    public class ReportFilter
    {
        public string maDViQLy { get; set; }
        public int quater { get; set; }
        public int year { get; set; } = DateTime.Now.Year;
    }
}