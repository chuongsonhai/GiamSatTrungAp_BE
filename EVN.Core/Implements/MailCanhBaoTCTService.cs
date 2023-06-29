using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class MailCanhBaoTCTService : FX.Data.BaseService<MailCanhBaoTCT, int>, IMailCanhBaoTCTService
    {
        private readonly ILog log = LogManager.GetLogger(typeof(MailCanhBaoTCTService));
        public MailCanhBaoTCTService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public IList<MailCanhBaoTCT> GetByFilter(string tenNV, string email, int pageindex, int pagesize, out int total)
        {
            var query = Query;
            
            if (!string.IsNullOrWhiteSpace(tenNV))
                query = query.Where(p => p.TENNHANVIEN.ToLower().Contains(tenNV.ToLower()));
            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(p => p.EMAIL.ToLower().Contains(email.ToLower()));
            

            query = query.OrderBy(p => p.TENNHANVIEN);
            int maxtemp = pageindex <= 1 ? 4 - pageindex : 2;//load tối đa 2 trang tiếp theo, nếu page =1 hoặc 2 thì sẽ load 4 trang hoặc 3 trang
            var ret = query.Skip(pageindex * pagesize).Take(pagesize * maxtemp + 1).ToList();
            total = pageindex * pagesize + ret.Count;
            return ret.Take(pagesize).ToList();
        }
    }
}
