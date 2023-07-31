using EVN.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
  
    public class XacNhanTroNgaiFilterkhaosatRequest : BaseRequest
    {

        [JsonProperty("filterdashboadkhaosat")]
        public dashboadkhaosat Filterdashboadkhaosat { get; set; }
    }

    public class dashboadkhaosat
    {

        public string tuNgay { get; set; }
        public string denNgay { get; set; }


    }

}
