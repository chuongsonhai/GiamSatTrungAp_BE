using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class AF_A_ASSET_ATT_ITEM
    {
        public virtual string AFFILEID { get; set; }
        public virtual string FILENAME { get; set; }
        public virtual decimal FILESIZEB { get; set; }
        public virtual DateTime CRDTIME { get; set; } = DateTime.Now;
        public virtual DateTime UPDDTIME { get; set; } = DateTime.Now;
        public virtual DateTime MDFDTIME { get; set; } = DateTime.Now;
        public virtual string FILETYPEID { get; set; } = "DOC";
        public virtual string ASSETID { get; set; }
        public virtual string SUMDESC { get; set; }
        public virtual string ATTTYPE { get; set; } = "FILE";
    }
}
