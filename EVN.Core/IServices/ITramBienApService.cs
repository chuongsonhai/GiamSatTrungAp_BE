using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface ITramBienApService : FX.Data.IBaseService<TramBienAp, int>
    {
        IList<TramBienAp> getByBienBanDNID(int id);
    }
}
