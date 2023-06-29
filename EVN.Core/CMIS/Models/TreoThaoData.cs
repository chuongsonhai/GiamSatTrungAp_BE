using System.Collections.Generic;

namespace EVN.Core.CMIS
{
    public class TreoThaoData
    {
        public CongToData CONG_TO { get; set; }
        public IList<TiData> TI { get; set; } = new List<TiData>();
        public IList<TuData> TU { get; set; } = new List<TuData>();
        public string HSNDD_TREO { get; set; }
        public bool CTO_TREO { get; set; } = true;
    }    
}
