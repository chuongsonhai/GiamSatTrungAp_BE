using EVN.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
  
    public class XacnhantrongaiFilterRequestid : BaseRequest
    {
        [JsonProperty("filter")]
        public XacnhantrongaiRequestid Filter { get; set; }
    }

    public class XacnhantrongaiRequestid
    {
        public int ID { get; set; }
    }

}
