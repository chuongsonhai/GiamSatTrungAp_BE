using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IThietBiService : FX.Data.IBaseService<ThietBi, int>
    {
        IList<ThietBi> GetbyMaYCau(string maYCau);
        IList<ThietBi> GetByFilter(string maDonViQL, string maYCau);
    }
}
