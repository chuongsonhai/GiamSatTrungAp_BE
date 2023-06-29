using EVN.Core.CMIS;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EVN.Core.Implements
{
    public class NhanVienService : FX.Data.BaseService<NhanVien, int>, INhanVienService
    {
        ILog log = LogManager.GetLogger(typeof(NhanVienService));
        public NhanVienService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public NhanVien GetbyCode(string maDViQLy, string maNVien)
        {
            return Get(p => p.MA_DVIQLY == maDViQLy && p.MA_NVIEN == maNVien);
        }

        public List<NhanVien> GetbyMaBPhan(string maDViQLy, string maBPhan)
        {
            string bphan = $"({maBPhan})";
            var query = Query.Where(p => p.MA_DVIQLY == maDViQLy && p.MA_BPHAN.Contains(bphan));            
            return query.ToList();
        }
        public IList<NhanVien> GetbyFilter(string maDVi, string maBPhan, string keyword, int pageindex, int pagesize, out int total)
        {
            var query = Query;
            if (!string.IsNullOrWhiteSpace(maDVi))
                query = query.Where(p => p.MA_DVIQLY == maDVi);
            if (!string.IsNullOrWhiteSpace(maBPhan))
            {
                maBPhan = $"({maBPhan})";
                query = query.Where(p => p.MA_BPHAN.Contains(maBPhan));
            }

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.MA_NVIEN.ToLower().Contains(keyword.ToLower()) || p.TEN_NVIEN.ToLower().Contains(keyword.ToLower()));
            total = query.Count();
            return query.Skip(pageindex * pagesize).Take(pagesize).ToList();
        }

        public void Sync(string maDViQLy)
        {
            try
            {
                ICmisProcessService cmisProcess = new CmisProcessService();
                var list = cmisProcess.GetNhanViens(maDViQLy);
                if (list != null && list.Count() >= 0)
                {
                    foreach (var item in list)
                    {
                        var nhanvien = GetbyCode(maDViQLy, item.MA_NVIEN);
                        if (nhanvien != null) continue;
                        nhanvien = new NhanVien();
                        nhanvien.MA_DVIQLY = maDViQLy;
                        
                        nhanvien.TEN_NVIEN = item.TEN_NVIEN;
                        nhanvien.MA_NVIEN = item.MA_NVIEN;
                        nhanvien.DIEN_THOAI = item.DIEN_THOAI;
                        nhanvien.EMAIL = item.EMAIL;
                        Save(nhanvien);
                    }
                    CommitChanges();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
