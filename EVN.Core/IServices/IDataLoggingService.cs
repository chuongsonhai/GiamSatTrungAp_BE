using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IDataLoggingService : FX.Data.IBaseService<DataLogging, int>
    {
        void CreateLog(string madonvi, string mayeucau,string userid, string username, string actiontype, string dataLog);
        IList<DataLogging> GetbyFilter(string madonvi,string mayeucau,string username, string keyword, DateTime fromtime, DateTime totime, int pageindex, int pagesize, out int total);
    }
}
