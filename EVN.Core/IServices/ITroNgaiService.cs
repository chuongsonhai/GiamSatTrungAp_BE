using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface ITroNgaiService : FX.Data.IBaseService<TroNgai, string>
    {
        IList<TroNgai> GetbyFilter(string keyword, int pageindex, int pagesize, out int total);
    }
}
