using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class TemplateService : FX.Data.BaseService<Template, string>, ITemplateService
    {
        public TemplateService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
    }
}
