using EVN.Core.Domain;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EVN.Core.IServices
{
    public interface INhanVienService : FX.Data.IBaseService<NhanVien, int>
    {
        List<NhanVien> GetbyMaBPhan(string maDViQLy, string maBPhan);

        IList<NhanVien> GetbyFilter(string maDVi, string maBPhan, string keyword, int pageindex, int pagesize, out int total);
        NhanVien GetbyCode(string maDViQLy, string maNVien);
        void Sync(string maDViQLy);
    }
}