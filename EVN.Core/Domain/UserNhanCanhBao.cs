using System;
namespace EVN.Core.Domain

{
    public class UserNhanCanhBao
    {
        public virtual int ID { get; set; }
        public virtual int USER_ID { get; set; }
        //  public virtual int TRANGTHAI_GUI { get; set; }
        public virtual string MA_DVIQLY { get; set; }
        public virtual int TRANGTHAI { get; set; }
    }

}
