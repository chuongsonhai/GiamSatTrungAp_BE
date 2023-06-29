using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IQuyMoCongTrinhService : FX.Data.IBaseService<QuyMoCongTrinh, int>
    {
        QuyMoCongTrinh GetByBienBanDNID(int id);
    }
}
