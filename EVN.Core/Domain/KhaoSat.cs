using System;
namespace EVN.Core.Domain

{
    public class KhaoSat
    {
        public virtual int ID { get; set; }
        public virtual int CANHBAO_ID { get; set; }
        public virtual string NOIDUNG_CAUHOI { get; set; }
        public virtual string PHANHOI_KH { get; set; } 
        public virtual DateTime THOIGIAN_KHAOSAT { get; set; }
        public virtual string NGUOI_KS { get; set; }
        public virtual string KETQUA { get; set; }
        public virtual string TINHTRANG_KT_CB { get; set; }
        public virtual string TRANGTHAI_XOA_KHAOSAT { get; set; }
        public virtual string DONVI_QLY { get; set; }
    }
}
