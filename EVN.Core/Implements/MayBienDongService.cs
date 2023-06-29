using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class MayBienDongService : FX.Data.BaseService<MayBienDong, int>, IMayBienDongService
    {
        public MayBienDongService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public IList<MayBienDong> GetByBBTTID(int BTTID, bool loai)
        {
            return Query.Where(x => x.BBAN_ID == BTTID && x.TI_THAO == loai).ToList();
        }
    }
}