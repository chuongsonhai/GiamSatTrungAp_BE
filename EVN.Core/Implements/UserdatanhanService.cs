using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class UserdatanhanService : FX.Data.BaseService<Userdatanhan, int>, IUserdatanhanService
    {
        private ILog log = LogManager.GetLogger(typeof(UserdataService)); 
        public UserdatanhanService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public IList<Userdatanhan> Getbyusernhan(string maDViQLy)
        {
            var query = Query;

            query = query.Where(p => "-1" == maDViQLy && p.maDViQLy != null);

            return query.ToList();
        }


    }
}
