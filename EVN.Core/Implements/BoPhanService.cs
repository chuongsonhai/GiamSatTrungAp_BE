using EVN.Core.CMIS;
using EVN.Core.Domain;
using EVN.Core.IServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EVN.Core.Implements
{
    public class BoPhanService : FX.Data.BaseService<BoPhan, int>, IBoPhanService
    {
        public BoPhanService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public BoPhan GetbyCode(string maDViQLy, string maBPhan)
        {
            return Get(p => p.MA_DVIQLY == maDViQLy && p.MA_BPHAN == maBPhan);
        }

        public IList<BoPhan> GetbyFilter(string maDVi, string keyword, int pageindex, int pagesize, out int total)
        {
            var query = Query;
            if (!string.IsNullOrWhiteSpace(maDVi))
                query = query.Where(p => p.MA_DVIQLY == maDVi);
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.MA_BPHAN.ToLower().Contains(keyword.ToLower()) || p.TEN_BPHAN.ToLower().Contains(keyword.ToLower()));
            total = query.Count();
            return query.Skip(pageindex * pagesize).Take(pagesize).ToList();
        }

        public IList<BoPhan> GetbyMaDVi(string maDVi, params string[] maBPhans)
        {
            var query = Query.Where(p => p.MA_DVIQLY == maDVi);
            if (maBPhans != null && maBPhans.Count() > 0)
                query = query.Where(p => maBPhans.Contains(p.MA_BPHAN));
            return query.ToList();
        }

        public void Sync(string maDViQLy)
        {
            try
            {
                ICmisProcessService cmisProcess = new CmisProcessService();
                var list = cmisProcess.GetBoPhans(maDViQLy);
                if (list != null && list.Count() >= 0)
                {
                    foreach (var item in list)
                    {
                        var bophan = GetbyCode(item.MA_DVIQLY, item.MA_BPHAN);
                        if (bophan != null) continue;
                        bophan = new BoPhan();
                        bophan.MA_DVIQLY = item.MA_DVIQLY;
                        bophan.MA_BPHAN = item.MA_BPHAN;
                        bophan.TEN_BPHAN = item.TEN_BPHAN;
                        bophan.MO_TA = item.MO_TA;
                        bophan.GHI_CHU = item.GHI_CHU;
                        Save(bophan);
                    }
                    CommitChanges();
                }
                list = cmisProcess.GetDSTo(maDViQLy);
                if (list != null && list.Count() >= 0)
                {
                    foreach (var item in list)
                    {
                        var bophan = GetbyCode(item.MA_DVIQLY, item.MA_BPHAN);
                        if (bophan != null) continue;
                        bophan = new BoPhan();
                        bophan.MA_DVIQLY = item.MA_DVIQLY;
                        bophan.MA_BPHAN = item.MA_BPHAN;
                        bophan.TEN_BPHAN = item.TEN_BPHAN;
                        bophan.MO_TA = item.MO_TA;
                        bophan.GHI_CHU = item.GHI_CHU;
                        Save(bophan);
                    }
                    CommitChanges();
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
