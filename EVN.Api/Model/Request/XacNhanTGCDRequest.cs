using EVN.Core;
using EVN.Core.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
 
        public class XacNhanTGCDReques : BaseRequest
        {

            [JsonProperty("filtertgcd")]
            public dashboadtgcd Filtertgcd { get; set; }
        }
        public class dashboadtgcd
    {
            public string tuNgay { get; set; }
            public string denNgay { get; set; }

            public string donViQuanLy { get; set; }
        }

    


}
