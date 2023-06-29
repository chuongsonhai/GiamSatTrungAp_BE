using EVN.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
    public class UserFilterRequest : BaseRequest
    {
        [JsonProperty("Filter")]
        public UserFilter Filter { get; set; }
    }

    public class UserFilter
    {
        public string keyword { get; set; } = string.Empty;
        public string maDViQLy { get; set; }
        public string maBPhan { get; set; }
        public bool dongBoCmis { get; set; } = false;
    }

    public class NhanVienFilter
    {
        public string maDonVi { get; set; }
        public string maBPhan { get; set; }
        public bool truongBPhan { get; set; } = false;
        public bool dongBoCmis { get; set; } = false;
    }
}
