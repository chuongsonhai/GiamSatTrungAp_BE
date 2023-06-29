using EVN.Core.Domain;
using FX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface ITemplateService : FX.Data.IBaseService<Template, string>
    {
    }

    public static class TemplateManagement
    {
        public static Template GetTemplate(string code)
        {
            try
            {
                ITemplateService service = IoC.Resolve<ITemplateService>();
                Template result = service.Getbykey(code);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
