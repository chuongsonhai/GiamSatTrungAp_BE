using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IHeThongDDChamDutService : FX.Data.IBaseService<HeThongDDChamDut, int>
    {
        IList<HeThongDDChamDut> GetByTTDBID(int ttdbid);
    }
}
