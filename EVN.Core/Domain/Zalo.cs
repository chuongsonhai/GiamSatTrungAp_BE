﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class Zalo
    {
        public virtual int ID { get; set; }
        public virtual string MA_DVIQLY { get; set; }
        public virtual string MA_KHANG { get; set; }
        public virtual string MA_DVU { get; set; }
        public virtual string ID_ZALO { get; set; }
        public virtual string NOI_DUNG { get; set; }
        public virtual int TINH_TRANG { get; set; }
        public virtual DateTime NGAY_TAO { get; set; }
        public virtual string NGUOI_TAO { get; set; }
        public virtual DateTime NGAY_SUA { get; set; }
        public virtual string NGUOI_SUA { get; set; }
        public virtual int ID_HDON { get; set; }
        public virtual string TIEU_DE { get; set; }
    }
}
