using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface ISendMailService : FX.Data.IBaseService<SendMail, int>
    {
        IList<SendMail> GetbyFilter(string maYCau, string keyword, int status, DateTime fromtime, DateTime totime, int pageindex, int pagesize, out int total);
        void Process(SendMail item, string templateName, Dictionary<string, string> bodyParams);
    }
}
