using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class SendMailData
    {
        public SendMailData() { }
        public SendMailData(SendMail entity) : base()
        {
            ID = entity.ID;
            TIEUDE = entity.TIEUDE;
            EMAIL = entity.EMAIL;
            NOIDUNG = entity.NOIDUNG;
            MA_YCAU_KNAI = entity.MA_YCAU_KNAI;
            MA_DVIQLY = entity.MA_DVIQLY;
            NGAY_TAO = entity.NGAY_TAO.ToString("dd/MM/yyyy");
            NGAY_GUI = entity.NGAY_GUI.ToString("dd/MM/yyyy");
            NGUOI_TAO = entity.NGUOI_TAO;
            TRANG_THAI = entity.TRANG_THAI;
        }
        public virtual int ID { get; set; }
        public virtual string EMAIL { get; set; }
        public virtual string TIEUDE { get; set; }
        public virtual string MA_YCAU_KNAI { get; set; }
        public virtual string MA_DVIQLY { get; set; }
        public virtual string NOIDUNG { get; set; }
        public virtual string NGAY_TAO { get; set; }
        public virtual string NGAY_GUI { get; set; }
        public virtual string NGUOI_TAO { get; set; }
        public virtual int TRANG_THAI { get; set; } = 0;
        public SendMail ToEntity(SendMail entity)
        {
     
            entity.TIEUDE = TIEUDE;
            entity.MA_YCAU_KNAI = MA_YCAU_KNAI;
            entity.MA_DVIQLY = MA_DVIQLY;
            entity.EMAIL=EMAIL;
            entity.NOIDUNG = NOIDUNG;
            if (!string.IsNullOrWhiteSpace(NGAY_TAO))
                entity.NGAY_TAO = DateTime.ParseExact(NGAY_TAO, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            if (!string.IsNullOrWhiteSpace(NGAY_GUI))
                entity.NGAY_GUI = DateTime.ParseExact(NGAY_GUI, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            entity.NGUOI_TAO = NGUOI_TAO;
            entity.TRANG_THAI = TRANG_THAI;
            return entity;
        }

    }
}