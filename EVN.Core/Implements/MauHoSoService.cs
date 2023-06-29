using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class MauHoSoService : FX.Data.BaseService<MauHoSo, string>, IMauHoSoService
    {
        public MauHoSoService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
    }
}
