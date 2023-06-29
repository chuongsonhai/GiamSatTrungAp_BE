using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class AF_A_ASSET_ATT_ITEM_FILE
    {
        public virtual string AFFILEID { get; set; }
        public virtual byte[] FILEDATA { get; set; }
    }
}
