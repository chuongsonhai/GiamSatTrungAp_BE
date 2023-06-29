namespace EVN.Core.Domain
{
    public class NhanVien
    {
        public virtual int ID { get; set; }
        public virtual string MA_DVIQLY { get; set; }
        public virtual string MA_BPHAN { get; set; }
        public virtual string MA_NVIEN { get; set; }
        public virtual string TEN_NVIEN { get; set; }

        public virtual string DIEN_THOAI { get; set; }
        public virtual string EMAIL { get; set; }
        public virtual bool TRUONG_BPHAN { get; set; } = false;
        public virtual string DIA_CHI { get; set; }
        public virtual string CVIEC { get; set; }
    }
}