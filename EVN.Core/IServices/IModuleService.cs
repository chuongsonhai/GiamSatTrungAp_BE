using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IModuleService : FX.Data.IBaseService<Module, string>
    {
        IList<Module> GetModules(bool? active);
    }
}
