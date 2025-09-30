using EVN.Core.CMIS;
using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Repository;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static iTextSharp.text.pdf.AcroFields;

namespace EVN.Core.Implements
{
    public class HoSoGiayToService : FX.Data.BaseService<HoSoGiayTo, int>, IHoSoGiayToService
    {
        ILog log = LogManager.GetLogger(typeof(HoSoGiayToService));
        public HoSoGiayToService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public HoSoGiayTo GetbyCode(string code)
        {
            return Get(p => p.MaHoSo == code);
        }

        public IList<HoSoGiayTo> GetbyYeuCau(string maDonVi, string maYeuCau)
        {
            var list = Query.Where(p => p.MaDViQLy == maDonVi && p.MaYeuCau == maYeuCau).OrderBy(p => p.ID).ToList();
            return list;
        }

        public IList<HoSoGiayTo> GetbyYeuCau2( string maYeuCau)
        {
            var list = Query.Where(p =>  p.MaYeuCau == maYeuCau).OrderBy(p => p.ID).ToList();
            return list;
        }

        public IList<HoSoGiayTo> GetbyYeuCaudup(string maDonVi, string maYeuCau)
        {
            var list = Query
           .Where(p => p.MaDViQLy == maDonVi && p.MaYeuCau == maYeuCau && p.LoaiHoSo.CompareTo("57") <= 0)
           .OrderBy(p => p.ID)
           .ToList();
            return list;
        }

        public IList<HoSoGiayTo> GetbyYeuCaudup2( string maYeuCau)
        {
            var list = Query
           .Where(p => p.MaYeuCau == maYeuCau && p.LoaiHoSo.CompareTo("57") <= 0)
           .OrderBy(p => p.ID)
           .ToList();
            return list;
        }

        public HoSoGiayTo GetHoSoGiayTo(string maDonVi, string maYCau, string loaiHSo)
        {
            return Get(p => p.MaDViQLy == maDonVi && p.MaYeuCau == maYCau && p.LoaiHoSo == loaiHSo);
        }

        public HoSoGiayTo GetHoSoGiayTo2( string maYCau, string loaiHSo)
        {
            return Get(p => p.MaYeuCau == maYCau && p.LoaiHoSo == loaiHSo);
        }

        public IList<HoSoGiayTo> ListHSoGTo(string maDonVi, string maYeuCau)
        {
            List<string> ignorecodes = new List<string>()
            { LoaiHSoCode.CV_DN, LoaiHSoCode.BB_KS, LoaiHSoCode.BB_DN, LoaiHSoCode.CV_NT, LoaiHSoCode.BB_KT, LoaiHSoCode.BB_NT, LoaiHSoCode.HD_NSH,LoaiHSoCode.HD_SH, LoaiHSoCode.DN_NT, LoaiHSoCode.BB_TT, LoaiHSoCode.PL_HD };
            ignorecodes.AddRange(new List<string>() { LoaiHSoCode.PL_HD_DB, LoaiHSoCode.PL_HD_MB, LoaiHSoCode.PL_HD_CD, LoaiHSoCode.PL_BDPT, LoaiHSoCode.PL_TB });
            //var items = Query.Where(p => p.MaDViQLy == maDonVi && p.MaYeuCau == maYeuCau && !ignorecodes.Contains(p.LoaiHoSo)).OrderBy(p => p.ID).ToList();
            //if (items.Count() > 0) return items;

            var items = new List<HoSoGiayTo>();
            ICmisProcessService processSrv = new CmisProcessService();
            HSoGToResult result = processSrv.GetlistHSoGTo(maDonVi, maYeuCau);
            if (result != null && result.TYPE == "OK")
            {                
                var listHso = result.HSO_GTO;
                log.Error($"listHso: {listHso.Count}");
                foreach (var item in listHso)
                {
                    log.Error($"MA_HSGT: {item.MA_HSGT}");
                    if (ignorecodes.Contains(item.MA_HSGT))
                    {
                        log.Error($"MA_HSGT: {item.MA_HSGT}");
                        continue;
                    }

                    HoSoGiayTo hoSo = GetHoSoGiayTo(item.MA_DVIQLY, item.MA_YCAU, item.MA_HSGT);
                    if (hoSo != null)
                    {
                        items.Add(hoSo);
                        continue;
                    }
                      

                    hoSo = new HoSoGiayTo();

                    hoSo.MaYeuCau = item.MA_YCAU;
                    hoSo.LoaiHoSo = item.MA_HSGT;
                    hoSo.TenHoSo = item.TEN_HSGT;
                    hoSo.MaDViQLy = item.MA_DVIQLY;
                    hoSo.TrangThai = int.Parse(item.TINH_TRANG);

                    string extFile = Path.GetExtension(item.DUONG_DAN);
                    hoSo.LoaiFile = !string.IsNullOrWhiteSpace(extFile) ? extFile.ToUpper().Replace(".","") : "PDF";
                    
                    CreateNew(hoSo);
                    items.Add(hoSo);
                }
                CommitChanges();
                log.Error($"item: {JsonConvert.SerializeObject(items)}");
                return items;
            }
            return items;
        }

        public IList<HoSoGiayTo> ListSign(string maDonVi, string keyword, int pageIndex, int pageSize, out int total)
        {
            var query = Query.Where(p => p.MaDViQLy == maDonVi && p.TrangThai == 1 && p.TrinhKy);
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.TenHoSo.Contains(keyword) || p.LoaiHoSo.Contains(keyword));
            total = query.Count();
            query = query.OrderBy(p => p.NgayTao);
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
