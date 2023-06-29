using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.CMIS
{
    public class TTinCDan
    {
        public string SO_CMND { get; set; }
        public string HOTEN_CDAN { get; set; }
        public string HOTEN_CDAN_U { get; set; }
        public string NGAY_SINH { get; set; }
        public bool KQUA_XMINH { get; set; } = false;
        public bool KQUA_XMINH_NEW { get; set; } = false;
    }
}
