using EVN.Core.Domain;
using EVN.Core.IServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EVN.Core.Implements
{
    public class DataLoggingService : FX.Data.BaseService<DataLogging, int>, IDataLoggingService
    {
        public DataLoggingService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public void CreateLog(string madonvi, string mayeucau, string userid, string username, string actiontype, string dataLog)
        {

            try
            {
                DataLogging data = new DataLogging();
                data.MaDViQLy = madonvi;
                data.MaYeuCau = mayeucau;
                data.UserID = userid;
                data.UserName = username;
                data.ActionType = actiontype;
                data.SourceType = "TrungAp";
                data.DataLoggingDetail = dataLog;
                CreateNew(data);
                CommitChanges();
            }
            catch (Exception ex)
            {

            }
        }


        public IList<DataLogging> GetbyFilter(string madonvi, string mayeucau, string username, string keyword, DateTime fromtime, DateTime totime, int pageindex, int pagesize, out int total)
        {
            var query = Query.Where(p => p.NgayUpdate >= fromtime && p.NgayUpdate < totime.AddDays(1));
            if (!string.IsNullOrWhiteSpace(madonvi))
                query = query.Where(p => p.MaDViQLy == madonvi);
            if (!string.IsNullOrWhiteSpace(mayeucau))
                query = query.Where(p => p.MaYeuCau == mayeucau);
            if (!string.IsNullOrWhiteSpace(username))
                query = query.Where(p => p.UserName == username);
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.ActionType.Contains(keyword) || p.DataLoggingDetail.Contains(keyword));
            int maxtemp = pageindex <= 1 ? 4 - pageindex : 2;
            var ret = query.OrderByDescending(p => p.NgayUpdate).Skip(pageindex * pagesize).Take(pagesize * maxtemp + 1).ToList();
            total = pageindex * pagesize + ret.Count;
            return ret.Take(pagesize).ToList();
        }
       
    }
}
