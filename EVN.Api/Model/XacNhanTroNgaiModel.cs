using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class XacNhanTroNgaiModelkhaosatid
    {

        public virtual int ID { get; set; }
        public virtual string NOIDUNG_CAUHOI { get; set; }
        public virtual DateTime THOIGIAN_KHAOSAT { get; set; } = DateTime.Now;
        public virtual string NGUOI_KS { get; set; }
        public virtual string KETQUA { get; set; }
        public virtual int TRANGTHAI { get; set; }

    }
   
}
