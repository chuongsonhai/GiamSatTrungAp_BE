using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface ICongVanYeuCauService : FX.Data.IBaseService<CongVanYeuCau, int>
    {
        void SyncbyDate(string maLoaiYCau, DateTime tuNgay, DateTime denNgay);

        CongVanYeuCau DongBo(CongVanYeuCau yeucau);
        void SyncHU();

        CongVanYeuCau SyncData(string maYCau);
        IList<CongVanYeuCau> GetbyFilter(string maDViQLy, string keyword, string khachhang, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total);
        IList<CongVanYeuCau> GetList(string maDViQLy, DateTime fromdate, DateTime todate, out int total);
        IList<CongVanYeuCau> GetThongKe(string maDViQLy, string keyword, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total);
        IList<CongVanYeuCau> GetThongKeExport(string maDViQLy, string keyword, DateTime fromdate, DateTime todate);
        bool CreateNew(CongVanYeuCau congvan, out string message);

        bool DuyetHoSo(CongVanYeuCau congvan, string maCViec, string bPhanNhan, string nVienNhan, DateTime ngayHen, string noiDung, out string message);

        bool YeuCauKhaoSat(CongVanYeuCau congvan, string maCViec, string bPhanNhan, string nVienNhan, DateTime ngayHen, string noiDung, out string message);

        bool Cancel(CongVanYeuCau yeucau);

        CongVanYeuCau GetbyMaYCau(string maYCau);

        bool ChuyenTiep(string maYCau, string maDViTNhan);
        bool CancelYeuCauKhaoSat(CongVanYeuCau congvan);
    }
}
