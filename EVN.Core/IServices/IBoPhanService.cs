using EVN.Core.Domain;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EVN.Core.IServices
{
    public interface IBoPhanService : FX.Data.IBaseService<BoPhan, int>
    {
        IList<BoPhan> GetbyFilter(string maDVi, string keyword, int pageindex, int pagesize, out int total);
        IList<BoPhan> GetbyMaDVi(string maDVi, params string[] maBPhans);
        BoPhan GetbyCode(string maDViQLy, string maBPhan);
        void Sync(string maDViQLy);
    }
}