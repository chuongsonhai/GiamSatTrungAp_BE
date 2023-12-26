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

        public Userdatanhan GetMaDviQly(string MaDviQly)
        {
            return Get(p => p.maDViQLy == MaDviQly);
        }

        public IList<Userdatanhan> Getid(string MaDviQly)
        {
            IUserdataService serviceuser = IoC.Resolve<IUserdataService>();
            var resultList = new List<Userdatanhan>();
            if (MaDviQly != "-1")
            {
                var query = Query.Where(p => p.maDViQLy == MaDviQly && p.maDViQLy != null);
                var listCanhBao = query.ToList();
                foreach (var item in listCanhBao)
                {
                    var userNhancb = new Userdatanhan();
                    var userdata = serviceuser.Getid(item.userId);
                    userNhancb.userId = userdata.userId;
                    userNhancb.username = userdata.username;
                    userNhancb.maDViQLy = userdata.maDViQLy;
                    resultList.Add(userNhancb);
                }

            }
            else
            {
                var query = Query.Where(p => "-1" == MaDviQly && p.maDViQLy != null);
                var listCanhBao = query.ToList();
                foreach (var item in listCanhBao)
                {
                    var userNhancb = new Userdatanhan();
                    var userdata = serviceuser.Getid(item.userId);
                    userNhancb.userId = userdata.userId;
                    userNhancb.username = userdata.username;
                    userNhancb.maDViQLy = userdata.maDViQLy;
                    //userNhancb.fullName = userdata.fullName;
                    resultList.Add(userNhancb);
                }
            }
            return resultList;
        }

    }
}
