using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Models;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class NgayNghiLeService : FX.Data.BaseService<NgayNghiLe, int>, INgayNghiLeService
    {
        ILog log = LogManager.GetLogger(typeof(CanhBaoService));
        public NgayNghiLeService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
        public NgayNghiLe GetNgayLe(string key)
        {
            return Get(p => p.KEY == key);
        }

    }

    }
