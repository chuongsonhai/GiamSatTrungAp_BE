using EVN.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{

    public class CanhBaoFilterRequestid : BaseRequest
    {
        [JsonProperty("filterid")]
        public canhbaoid Filterid { get; set; }
    }

    public class canhbaoid
    {

        public int id { get; set; } = 0;

    }

}
