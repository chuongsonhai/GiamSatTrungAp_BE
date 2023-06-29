using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class KeyStoresService : FX.Data.BaseService<KeyStores, int>, IKeyStoresService
    {
        public KeyStoresService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public KeyStores GetActived()
        {
            return Get(p => p.IsActive);
        }
    }
}
