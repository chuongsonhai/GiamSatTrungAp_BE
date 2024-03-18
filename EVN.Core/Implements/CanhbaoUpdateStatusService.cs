using EVN.Core.CMIS;
using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Repository;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Core.Implements
{
    public class CanhbaoUpdateStatusService : FX.Data.BaseService<cbUpdateStatusModel, int>, ICanhbaoUpdateStatusService
    {
        ILog log = LogManager.GetLogger(typeof(CanhbaoUpdateStatusService));
        public CanhbaoUpdateStatusService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
      
    }
}