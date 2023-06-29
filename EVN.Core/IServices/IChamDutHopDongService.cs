using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IChamDutHopDongService : FX.Data.IBaseService<ChamDutHopDong, int>
    {
        ChamDutHopDong GetbyMaYCau(string maYCau);
        ChamDutHopDong GetbyCongvan(int congvanid);
        ChamDutHopDong CreateNew(ChamDutHopDong item, IList<HeThongDDChamDut> hethongdodem, out string message);
        ChamDutHopDong Update(ChamDutHopDong item, IList<HeThongDDChamDut> hethongdodem, out string message);

        bool Sign(ChamDutHopDong item);
    }
}
