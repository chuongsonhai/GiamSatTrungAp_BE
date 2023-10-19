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

    }
}