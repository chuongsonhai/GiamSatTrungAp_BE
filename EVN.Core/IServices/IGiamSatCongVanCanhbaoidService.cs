using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IGiamSatCongVanCanhbaoidService : FX.Data.IBaseService<GiamSatCongVanCanhbaoid, int>
    {
        //void SyncbyDate(string maLoaiYCau, DateTime tuNgay, DateTime denNgay);

        //GiamSatCongVanCanhbaoid DongBo(GiamSatCongVanCanhbaoid yeucau);

        //GiamSatCongVanCanhbaoid SyncData(string maYCau);
        //IList<GiamSatCongVanCanhbaoid> GetbyFilter(string maDViQLy, string keyword, string khachhang, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total);
        //IList<GiamSatCongVanCanhbaoid> GetList(string maDViQLy, DateTime fromdate, DateTime todate, out int total);
        //IList<GiamSatCongVanCanhbaoid> GetThongKe(string maDViQLy, string keyword, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total);
        //IList<GiamSatCongVanCanhbaoid> GetThongKeExport(string maDViQLy, string keyword, DateTime fromdate, DateTime todate);
        //bool CreateNew(GiamSatCongVanCanhbaoid congvan, out string message);

        //bool YeuCauKhaoSat(GiamSatCongVanCanhbaoid congvan, string maCViec, string bPhanNhan, string nVienNhan, DateTime ngayHen, string noiDung, out string message);

        //bool Cancel(GiamSatCongVanCanhbaoid yeucau);

        GiamSatCongVanCanhbaoid GetbyMaYCau(string maYCau);
        IList<GiamSatCongVanCanhbaoid> Filterkhaosat(string maycau);
        bool CancelYeuCauKhaoSat(GiamSatCongVanCanhbaoid congvan);
    }
}
