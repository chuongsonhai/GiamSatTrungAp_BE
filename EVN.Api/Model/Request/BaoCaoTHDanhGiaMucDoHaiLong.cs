using EVN.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model.Request
{

    public class BaoCaoTHDanhGiaMucDoHaiLong : BaseRequest
    {
        public FilterDanhGiaMucDoHaiLong FilterDGiaDoHaiLong { get; set; }

        public class FilterDanhGiaMucDoHaiLong
        {
            public string fromdate { get; set; }
            public string todate { get; set; }
        }
    }
}