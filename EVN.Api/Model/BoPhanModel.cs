using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class BoPhanModel
    {
        public int ID { get; set; }
        public string MA_DVIQLY { get; set; }
        public string MA_BPHAN { get; set; }
        public string TEN_BPHAN { get; set; }
        public string MO_TA { get; set; }
        public string GHI_CHU { get; set; }
        public List<string> CongViecs { get; set; } = new List<string>();
    }

    public class TroNgaiModel
    {
        public string MA_TNGAI { get; set; }
        public string TEN_TNGAI { get; set; }
        public List<string> CongViecs { get; set; } = new List<string>();
    }
}