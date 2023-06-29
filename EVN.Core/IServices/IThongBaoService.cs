using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IThongBaoService : FX.Data.IBaseService<ThongBao, int>
    {
        ThongBao GetbyYCau(string maDViQLy, string maYCau, LoaiThongBao loaiTBao);
        IList<ThongBao> GetbyNVien(string maDViQLy, string maNVien, DateTime fromdate, DateTime todate, int limit = 10);
        IList<ThongBao> GetbyFilter(string maDVi, string maNVien, string maYCau, int status, int pageindex, int pagesize, out int total);
    }
}
