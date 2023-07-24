using EVN.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
  
    public class CanhBaoFilterRequest : BaseRequest
    {
        [JsonProperty("filter")]
        public CanhBaoFilter Filter { get; set; }
    }

    public class CanhBaoFilter
    {
        public string DONVI_DIENLUC { get; set; } = string.Empty;
    }

}
