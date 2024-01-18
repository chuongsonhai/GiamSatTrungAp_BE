using EVN.Core.CMIS;
using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Models;
using EVN.Core.Repository;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace EVN.Core.Implements
{
    public class UserNhanCanhBaoService : FX.Data.BaseService<UserNhanCanhBao, int>, IUserNhanCanhBaoService
    {
        ILog log = LogManager.GetLogger(typeof(UserNhanCanhBaoService));
        public UserNhanCanhBaoService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public IList<UserNhanCanhBao> GetbyMaDviQly(string MaDviQly)
        {
            if(MaDviQly != "-1")
            {
                return Query.Where(p => p.MA_DVIQLY == MaDviQly || p.MA_DVIQLY == "X0206" || p.MA_DVIQLY == "PD").ToList();
            } else
            {
                return Query.ToList();
            }
            
        }


        

        public UserNhanCanhBao GetMaDviQly(string MaDviQly)
        {
                return Get(p => p.MA_DVIQLY == MaDviQly);
        }

        public UserNhanCanhBao GetbyNo(int idloai)
        {
            return Get(p => p.ID == idloai);
        }

        public IList<UserNhanCanhBao> Getid(string MaDviQly)
        {
            IUserdataService serviceuser = IoC.Resolve<IUserdataService>();
            var resultList = new List<UserNhanCanhBao>();
            if (MaDviQly != "-1")
            {
                var query = Query.Where(p => p.MA_DVIQLY == MaDviQly && p.MA_DVIQLY != null);
                var listCanhBao = query.ToList();
                foreach (var item in listCanhBao)
                {
                    var userNhancb = new UserNhanCanhBao();
                    var userdata = serviceuser.GetMaDviQly(item.MA_DVIQLY);
                    var userdataNHAN = serviceuser.Getid(item.USER_ID);
                    userdata.userId = userdataNHAN.userId;
                    if (userdata.userId == userdataNHAN.userId && userdataNHAN.maDViQLy == MaDviQly)
                    {
                        userNhancb.ID = item.ID;
                        userNhancb.USER_ID = userdataNHAN.userId;
                        userNhancb.USERNAME = userdataNHAN.username;
                        userNhancb.MA_DVIQLY = userdataNHAN.maDViQLy;
                        resultList.Add(userNhancb);
                    }
                }

            }
            else
            {
                var query = Query.Where(p => "-1" == MaDviQly && p.MA_DVIQLY != null);
                var listCanhBao = query.ToList();
                foreach (var item in listCanhBao)
                {
                    var userNhancb = new UserNhanCanhBao();
                    var userdata = serviceuser.GetMaDviQly(item.MA_DVIQLY);
                    var userdataNHAN = serviceuser.Getid(item.USER_ID);
                    userdata.userId = userdataNHAN.userId;
                    if (userdata.userId == userdataNHAN.userId)
                    {
                    userNhancb.ID = item.ID;
                        userNhancb.USER_ID = userdataNHAN.userId;
                        userNhancb.USERNAME = userdataNHAN.username;
                        userNhancb.MA_DVIQLY = userdataNHAN.maDViQLy;
                        resultList.Add(userNhancb);
                    }
                }
            }
            return resultList;
        }

    }
}