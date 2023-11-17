using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EVN.Core.Implements
{
    public class ThongBaoService : FX.Data.BaseService<ThongBao, int>, IThongBaoService
    {
        public ThongBaoService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public IList<ThongBao> GetbyFilter(string maDVi, string maNVien, string maYCau, int status, int pageindex, int pagesize, out int total)
        {
            if (maDVi == "PD")
            {
                var query = Query.Where(p => p.MaDViQLy == maDVi);
                if (!string.IsNullOrWhiteSpace(maYCau))
                    query = query.Where(p => p.MaYeuCau == maYCau);

                if (!string.IsNullOrWhiteSpace(maNVien))
                    query = query.Where(p => p.NguoiNhan == maNVien);

                if (status > -1)
                    query = query.Where(p => p.TrangThai == (TThaiThongBao)status);
                total = query.Count();
                query = query.OrderByDescending(p => p.NgayTao).ThenBy(p => p.TrangThai);
                return query.Skip(pageindex * pagesize).Take(pagesize).ToList();
            }

            if (maDVi == "X0206")
            {
                var query = Query.Where(p => p.MaDViQLy == "PD");
                if (!string.IsNullOrWhiteSpace(maYCau))
                    query = query.Where(p => p.MaYeuCau == maYCau);

                if (!string.IsNullOrWhiteSpace(maNVien))
                    query = query.Where(p => p.NguoiNhan == maNVien);

                if (status > -1)
                    query = query.Where(p => p.TrangThai == (TThaiThongBao)status);
                total = query.Count();
                query = query.OrderByDescending(p => p.NgayTao).ThenBy(p => p.TrangThai);
                return query.Skip(pageindex * pagesize).Take(pagesize).ToList();
            }
            else
            {
        
                    var query = Query.Where(p => p.MaDViQLy == maDVi);
                    if (!string.IsNullOrWhiteSpace(maYCau))
                        query = query.Where(p => p.MaYeuCau == maYCau);

                    if (!string.IsNullOrWhiteSpace(maNVien))
                        query = query.Where(p => p.NguoiNhan == maNVien);

                    if (status > -1)
                        query = query.Where(p => p.TrangThai == (TThaiThongBao)status);
                    total = query.Count();
                    query = query.OrderByDescending(p => p.NgayTao).ThenBy(p => p.TrangThai);
                    return query.Skip(pageindex * pagesize).Take(pagesize).ToList();
             
            }
        }

        public IList<ThongBao> GetbyNVien(string maDViQLy, string maNVien, DateTime fromdate, DateTime todate, int limit = 10)
        {
            var query = Query.Where(p => p.MaDViQLy == maDViQLy && p.NguoiNhan == maNVien 
                && p.NgayTao >= fromdate && p.NgayTao < todate.AddDays(1) 
                && p.TrangThai == TThaiThongBao.Moi);
            query = query.OrderByDescending(p => p.NgayTao);
            return query.Take(limit).ToList();
        }

        public ThongBao GetbyYCau(string maDViQLy, string maYCau, LoaiThongBao loaiTBao)
        {
            return Get(p => p.MaDViQLy == maDViQLy && p.MaYeuCau == maYCau && p.Loai == loaiTBao);
        }
    }
}
