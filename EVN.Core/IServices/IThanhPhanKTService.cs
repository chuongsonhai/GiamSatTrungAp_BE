using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IThanhPhanKTService : FX.Data.IBaseService<ThanhPhanKT, int>
    {
        List<ThanhPhanKT> GetByBienBanID(int bienbanid);
    }
}
