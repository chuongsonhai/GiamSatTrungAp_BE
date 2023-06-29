using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IMayBienDongService : FX.Data.IBaseService<MayBienDong, int>
    {
        IList<MayBienDong> GetByBBTTID(int BTTID,bool loai);
    }
}