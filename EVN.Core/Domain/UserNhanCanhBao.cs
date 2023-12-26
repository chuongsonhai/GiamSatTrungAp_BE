using Newtonsoft.Json;
using System;
namespace EVN.Core.Domain

{
    public class UserNhanCanhBao
    {
        public virtual int ID { get; set; }
        public virtual int USER_ID { get; set; }
        //  public virtual int TRANGTHAI_GUI { get; set; }
        public virtual string MA_DVIQLY { get; set; }
        public virtual string USERNAME { get; set; }
        public virtual int TRANGTHAI { get; set; } = 0;
    }

    public class UserNhanCanhBaoFilterRequest : BaseRequest
    {
        [JsonProperty("filter")]
        public UserNhanCanhBaoFilter Filter { get; set; }
    }

    public class UserNhanCanhBaoFilter
    {
        public string maDViQLy { get; set; } = string.Empty;
        public int userid { get; set; } = 0;
    }

}
