using System;
namespace EVN.Core.Domain

{
    public class CanhBao
    {
        public virtual int ID { get; set; }
        public virtual int LOAI_CANHBAO_ID { get; set; }
        //  public virtual int TRANGTHAI_GUI { get; set; }
        public virtual DateTime THOIGIANGUI { get; set; } = DateTime.Now;
        public virtual string NOIDUNG { get; set; }
        public virtual int LOAI_SOLANGUI { get; set; }
        public virtual string MA_YC { get; set; }
        public virtual int TRANGTHAI_CANHBAO { get; set; }
        public virtual string DONVI_DIENLUC { get; set; }
        public virtual int NGUYENHHAN_CANHBAO { get; set; }
        public virtual string KETQUA_GIAMSAT { get; set; }
        public virtual string NGUOI_GIAMSAT { get; set; }
    }

}
