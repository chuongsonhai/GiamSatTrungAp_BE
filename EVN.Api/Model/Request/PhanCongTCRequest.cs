using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class PhanCongTCRequest
    {
        public string maYeuCau { get; set; }
        public int loai { get; set; } = 1;
    }
}