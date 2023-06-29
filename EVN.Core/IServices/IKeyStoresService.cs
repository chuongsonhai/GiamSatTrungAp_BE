using EVN.Core.Domain;
using FX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IKeyStoresService : FX.Data.IBaseService<KeyStores, int>
    {
        KeyStores GetActived();
    }
    public static class KeyStoresManagement
    {
        public static KeyStores GetKeyStore()
        {
            try
            {
                IKeyStoresService keyStoresSrv = IoC.Resolve<IKeyStoresService>();
                KeyStores result = keyStoresSrv.GetActived();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
