using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface ICauHinhDKService : FX.Data.IBaseService<CauHinhDK, int>
    {        
        IList<CauHinhDK> GetbyMaCViec(string maLoaiYCau, string maCViec);
    }
}
