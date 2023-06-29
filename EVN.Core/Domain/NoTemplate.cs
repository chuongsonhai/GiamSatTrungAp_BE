using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class NoTemplate
    {
        public virtual int ID { get; set; }
        public virtual decimal CurrentNo { get; set; } = 0;
        public virtual int CurrentYear { get; set; } = DateTime.Today.Year;
        public virtual string Format { get; set; }
        public virtual int Status { get; set; }
        public virtual NoType Type { get; set; } = NoType.BBKhaoSat;
    }

    public enum NoType
    {
        BBKhaoSat = 0,
        TTDauNoi = 1,
        BBKTDongDien = 2,
        CVYeuCauDN = 7,
        CVYeuCauNT = 8,
        HopDong=3,
        BBTreoThao = 4,
        PMIS=9,
    }
}
