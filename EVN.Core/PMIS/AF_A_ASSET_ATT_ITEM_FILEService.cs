using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.PMIS
{
    public class AF_A_ASSET_ATT_ITEM_FILEService : FX.Data.BaseService<AF_A_ASSET_ATT_ITEM_FILE, int>, IAF_A_ASSET_ATT_ITEM_FILEService
    {
        public AF_A_ASSET_ATT_ITEM_FILEService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
    }
}
