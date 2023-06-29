using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.PMIS
{
    public interface IAF_A_ASSET_ATT_ITEMService : FX.Data.IBaseService<AF_A_ASSET_ATT_ITEM, int>
    {
        bool DongBoPMIS(YCauNghiemThu yeucau);       
    }
}
