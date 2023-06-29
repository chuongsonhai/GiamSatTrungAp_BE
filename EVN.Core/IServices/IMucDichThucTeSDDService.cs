using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IMucDichThucTeSDDService : FX.Data.IBaseService<MucDichThucTeSDD, int>
    {
        IList<MucDichThucTeSDD> GetByTTDBID(int ttdbid);
    }
}
