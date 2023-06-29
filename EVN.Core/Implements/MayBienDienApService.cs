using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class MayBienDienApService : FX.Data.BaseService<MayBienDienAp, int>, IMayBienDienApService
    {
        public MayBienDienApService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public IList<MayBienDienAp> GetByBBTTID(int BTTID, bool loai)
        {
            return Query.Where(x=>x.BBAN_ID == BTTID && x.TU_THAO==loai).ToList();
        }
    }
}