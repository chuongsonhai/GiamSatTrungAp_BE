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
    public class MaDichVuService : FX.Data.BaseService<MaDichVu, string>, IMaDichVuService
    {
        private readonly ILog log = LogManager.GetLogger(typeof(MaDichVuService));
        public MaDichVuService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }                
    }
}
