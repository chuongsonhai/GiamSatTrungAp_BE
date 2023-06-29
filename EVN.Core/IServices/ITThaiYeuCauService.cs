using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface ITThaiYeuCauService : FX.Data.IBaseService<TThaiYeuCau, string>
    {
        TThaiYeuCau GetbyStatus(int status, int loai = 0);
    }
}
