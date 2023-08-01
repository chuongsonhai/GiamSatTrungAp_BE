using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class EmailModelFilter
    {
        public EmailModel Filter { get; set; }
    }
    public class EmailModel
    {
        public int ID { get; set; }
        public string MA_DVIQLY { get; set; }
        public string MA_KHANG { get; set; }
        public char MA_DVU { get; set; }
        public char EMAIL { get; set; }
        public char NOI_DUNG { get; set; }
        public int TINH_TRANG { get; set; }
        public DateTime NGAY_TAO { get; set; }
        public char NGUOI_TAO { get; set; }
        public DateTime NGAY_SUA { get; set; }
        public char NGUOI_SUA { get; set; }
        public int ID_HDON { get; set; }
        public char TIEU_DE { get; set; }


    }
}
