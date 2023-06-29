using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IYCauNghiemThuService : FX.Data.IBaseService<YCauNghiemThu, int>
    {
        void Sync();
        bool CreateNew(YCauNghiemThu congvan, out string message);
        IList<YCauNghiemThu> GetbyFilter(string maDViQLy,string keyword, string khachhang, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total);
        IList<YCauNghiemThu> GetList(string maDViQLy, DateTime fromdate, DateTime todate, out int total);
        bool Approve(YCauNghiemThu congvan, string maCViec, string bPhanNhan, string nVienNhan, DateTime ngayHen, string noiDung, out string message);

        bool YeuCauKiemTra(YCauNghiemThu congvan, string maCViec, string bPhanNhan, string nVienNhan, DateTime ngayHen, string noiDung, out string message);
        YCauNghiemThu GetbyMaYCau(string maYCau);

        YCauNghiemThu SyncData(YCauNghiemThu item);
        bool CancelYeuCauKiemTra(YCauNghiemThu congvan);

        void SyncPMIS();
        bool Cancel(YCauNghiemThu congvan);
    }
}
