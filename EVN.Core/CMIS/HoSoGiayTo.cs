using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.CMIS
{
    public class HSoGiayTo
    {
        public string MA_HSGT { get; set; }
        public string DUONG_DAN { get; set; }
        public string MA_YCAU { get; set; }
        public string TINH_TRANG { get; set; }
        public string MA_DVIQLY { get; set; }
        public string TEN_HSGT { get; set; }
    }

    public class HSoGToRequest
    {
        public string AccessKey { get; set; } = "123";
        public string SecretKey { get; set; } = "123";
        public string strMa_Dviqly { get; set; }
        public string strMa_YCau { get; set; }
        public string TYPE { get; set; } = "53";
    }

    public class HSoGToResult
    {
        public string MESSAGE { get; set; }
        public string TYPE { get; set; }
        public IList<HSoGiayTo> HSO_GTO { get; set; } = new List<HSoGiayTo>();
    }
}
