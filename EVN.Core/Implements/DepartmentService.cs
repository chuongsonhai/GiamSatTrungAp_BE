using EVN.Core.Domain;
using EVN.Core.IServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EVN.Core.Implements
{
    public class DepartmentService : FX.Data.BaseService<Department, long>, IDepartmentService
    {
        public DepartmentService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public IList<Department> GetbyOrgId(long orgId)
        {
            return Query.Where(p => p.orgId == orgId).ToList();
        }

        public IList<Department> ListbyParentID(string keyword, long parentID)
        {
            IList<Department> list = Query.Where(c => c.status == 1).ToList();            
            if (!string.IsNullOrWhiteSpace(keyword))
                list = list.Where(p => p.shortName.Contains(keyword) || p.name.Contains(keyword)).ToList();

            if (parentID > 0)
                list = list.Where(p => p.deptRoot == parentID).ToList();            
            return list;
        }

        public Department GetbyCode(string code)
        {
            return Get(p => p.shortName == code);
        }        

        public IList<Department> GetbyParentID(string keyword, int status, long parentID)
        {
            var query = Query;
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.shortName.Contains(keyword) || p.name.Contains(keyword));

            if (parentID > 0)
                query = query.Where(p => p.deptRoot == parentID);

            if (status > -1)
                query = query.Where(p => p.status == status);            
            return query.ToList();
        }

        public IList<Department> Sync()
        {
            SyncModel model = new SyncModel(null);
            string data = JsonConvert.SerializeObject(model);
            var result = ApiHelper.SyncApi("Department/getByUpdateTime");
            if (result == null) return new List<Department>();
            //var list = result.data.ToObject<List<Department>>();
            //foreach (var item in list)
            //{
            //    Save(item);
            //}
            //CommitChanges();
            return new List<Department>();
        }
    }

    public class SyncModel
    {
        public SyncModel(DateTime? update_time)
        {
            if (update_time.HasValue)
                UPDATE_TIME = update_time.Value.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public string UPDATE_TIME { get; set; } = string.Empty;
    }
}
