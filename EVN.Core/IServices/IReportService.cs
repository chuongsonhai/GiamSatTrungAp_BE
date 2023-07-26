using EVN.Core.Domain;
using EVN.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IReportService : FX.Data.IBaseService<CongVanYeuCau, int>
    {
        IList<ReportData> ListbyQuater(string maDViQLy, int quater, int year, int pageindex, int pagesize, out int total);
        IList<BaoCaoTongHopChiTietData> GetBaoCaoTongHop(DateTime tuNgay, DateTime denNgay, bool isHoanTat);
        IList<BaoCaoTongHopChiTietData> GetBaoCaoChiTiet(DateTime tuNgay, DateTime denNgay, bool isHoanTat);
        IList<BaoCaoTongHopChiTietData> GetBaoCaoChiTietLuyKe(DateTime tuNgay, DateTime denNgay, bool isHoanTat);
        IList<BaoCaoTTDN> GetListBaoCaoTTDN(string maDViQLy, string keyword, string khachhang, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total);
        IList<BaoCaoYCNT> GetListBaoCaoNT(string maDViQLy, string keyword, string khachhang, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total);
        IList<BaoCaoChiTietTCDN> GetListBaoCaoChiTietTCDN(string maDViQLy, string keyword, string khachhang, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total);
        IList<ThoiGianCapDienModel> GetThoigiancapdien(string maDViQLy, DateTime fromdate, DateTime todate);
        IList<BaoCaoTHTCDN> GetListBaoCaoTHTCDN(string maDViQLy, string keyword, string khachhang, int status, DateTime fromdate, DateTime todate);
        IList<BaoCaoChiTietTCDN> GetListBaoCaoChiTietQuaHan(string maDViQLy, string keyword, string khachhang, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total);
        IList<BaoCaoTHQuaHan> GetListBaoCaoTHQuaHan(string maDViQLy, string keyword, string khachhang, int status, DateTime fromdate, DateTime todate);
        IList<CongVanYeuCau> TinhThoiGian();
    }
}