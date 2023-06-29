using EVN.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class TienTrinhFilterRequest : BaseRequest
    {
        [JsonProperty("Filter")]
        public TienTrinhFilter Filter { get; set; }
    }

    public class TienTrinhFilter
    {
        public string keyword { get; set; }
        public string maYCau { get; set; }
    }
}