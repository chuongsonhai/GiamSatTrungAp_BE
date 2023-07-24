using System;
namespace EVN.Core.Domain

{
    public class GiamsatCanhbaoCanhbaoid
    {
        public virtual int idCanhBao { get; set; }
        public virtual int maLoaiCanhBao { get; set; }
        //  public virtual int TRANGTHAI_GUI { get; set; }
        public virtual string noidungCanhBao { get; set; }
        public virtual DateTime thoiGianGui { get; set; } = DateTime.Now;
        //public virtual int LOAI_SOLANGUI { get; set; }
      
        public virtual string donViQuanLy { get; set; }
        public virtual int trangThai { get; set; }
        public virtual string MA_YC { get; set; }
    }

}
