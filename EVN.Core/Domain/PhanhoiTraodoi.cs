﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class PhanhoiTraodoi
    {
        public virtual int ID { get; set; }
        public virtual int CANHBAO_ID { get; set; }
        public virtual string NOIDUNG_PHANHOI { get; set; } 
        public virtual string NGUOI_GUI { get; set; }
        public virtual string DONVI_QLY { get; set; }
        public virtual DateTime THOIGIAN_GUI { get; set; } = DateTime.Now;
        public virtual int TRANGTHAI_XOA { get; set; }
        public virtual string FILE_DINHKEM { get; set; }
        public virtual string MA_YC { get; set; }
        public virtual string NOIDUNG_PHANHOI_X3 { get; set; }
        public virtual string NGUOI_PHANHOI_X3 { get; set; }
    }
}
