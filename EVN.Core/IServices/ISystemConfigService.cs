using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface ISystemConfigService : FX.Data.IBaseService<SystemConfig, int>
    {
        SystemConfig GetbyCode(string code);

        IDictionary<string, string> GetDictionary(string charStart);
    }
}
