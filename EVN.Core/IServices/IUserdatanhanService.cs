using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IUserdatanhanService : FX.Data.IBaseService<Userdatanhan, int>
    {
        IList<Userdatanhan> Getbyusernhan(string maDViQLy);
    }
   
}
