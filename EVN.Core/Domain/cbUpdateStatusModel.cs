using System;
namespace EVN.Core.Domain

{
    public class cbUpdateStatusModel
    {
        public virtual int ID { get; set; }
        public virtual int NGUYENHHAN_CANHBAO { get; set; }
        public virtual int TRANGTHAI_CANHBAO { get; set; }
        public virtual string KETQUA_GIAMSAT { get; set; }
        public virtual string NGUOI_GIAMSAT { get; set; }
    }

}
