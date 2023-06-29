using EVN.Core;
using Newtonsoft.Json;

namespace EVN.Api.Model.Request
{
    public class ThongBaoRequest : BaseRequest
    {
        [JsonProperty("Filter")]
        public ThongBaoFilter Filter { get; set; }
    }

    public class ThongBaoFilter
    {
        public string maDViQLy { get; set; }
        public string maYCau { get; set; }
        public int status { get; set; } = 0;
    }
}