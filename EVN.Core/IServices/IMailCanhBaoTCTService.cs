using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IMailCanhBaoTCTService : FX.Data.IBaseService<MailCanhBaoTCT, int>
    {
        IList<MailCanhBaoTCT> GetByFilter(string tenNV,string email , int pageindex, int pagesize, out int total);
    }
}
