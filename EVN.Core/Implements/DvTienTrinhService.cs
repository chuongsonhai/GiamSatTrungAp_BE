using EVN.Core.CMIS;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using static iTextSharp.text.pdf.AcroFields;

namespace EVN.Core.Implements
{
    public class DvTienTrinhService : FX.Data.BaseService<DvTienTrinh, long>, IDvTienTrinhService
    {
        private ILog log = LogManager.GetLogger(typeof(DvTienTrinhService));
        public DvTienTrinhService(string sessionFactoryConfigPath) : base(sessionFactoryConfigPath)
        {
        }

        public bool PushToCmis(IList<DvTienTrinh> items, out string message)
        {
            try
            {
                message = "";
                ICongViecService cviecsrv = IoC.Resolve<ICongViecService>();
                CMISAction action = new CMISAction();
                foreach (var item in items.OrderBy(p => p.NGAY_BDAU).ThenBy(p => p.STT).ToList())
                {
                    if (item.NGAY_KTHUC.HasValue)
                    {
                        TimeSpan variable = item.NGAY_KTHUC.Value.Date - item.NGAY_BDAU.Date;
                        var songay = Math.Round(variable.TotalDays, 1, MidpointRounding.AwayFromZero) + 1;
                        item.SO_NGAY_LVIEC = songay.ToString();
                    }
                    if (string.IsNullOrWhiteSpace(item.NDUNG_XLY) || string.IsNullOrWhiteSpace(item.MA_TNGAI))
                    {
                        var congviec = cviecsrv.Getbykey(item.MA_CVIEC);
                        item.NDUNG_XLY = congviec.TEN_CVIEC;
                    }
                    IList<TienTrinhRequest> tientrinhs = new List<TienTrinhRequest>() { new TienTrinhRequest(item) };
                    string data = JsonConvert.SerializeObject(tientrinhs);
                    ApiService service = IoC.Resolve<ApiService>();
                    var result = service.PostData(action.themDvTienTrinh, data);
                    if (result == null) return false;
                    var response = JsonConvert.DeserializeObject<ApiResponse>(result);
                    if (response != null && (response.TYPE == "OK" || response.TYPE == "SUCCESS"))
                        item.TRANG_THAI = 1;
                    else
                        item.TRANG_THAI = 2;
                    Save(item);
                }
                CommitChanges();
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                message = ex.Message;
                return false;
            }
        }

        public IList<DvTienTrinh> GetbyFilter(string maYCau, string keyword, int pageindex, int pagesize, out int total)
        {
            var query = Query;
            if (!string.IsNullOrWhiteSpace(maYCau))
                query = query.Where(p => p.MA_YCAU_KNAI == maYCau);
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.NDUNG_XLY.Contains(keyword));
            total = query.Count();
            return query.OrderBy(p => p.STT).Skip(pageindex * pagesize).Take(pagesize).ToList();
        }

        public IList<DvTienTrinh> GetForExport(string maYCau, string keyword)
        {
            var query = Query;
            if (!string.IsNullOrWhiteSpace(maYCau))
                query = query.Where(p => p.MA_YCAU_KNAI == maYCau);
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.NDUNG_XLY.Contains(keyword));

            return query.OrderBy(p => p.NGAY_BDAU).ThenBy(p => p.KQ_ID_BUOC).ToList();
        }

        public DvTienTrinh GetbyYCau(string maYCau, string maCViec, int trangThai)
        {
            var query = Query.Where(p => p.MA_YCAU_KNAI == maYCau && p.MA_CVIEC == maCViec);
            if (trangThai >= 0)
                query = query.Where(p => p.TRANG_THAI == trangThai);
            query = query.OrderBy(p => p.TRANG_THAI);
            return query.FirstOrDefault();
        }

        public void DongBoTienDo(YCauNghiemThu yeucau)
        {
            try
            {
                ICauHinhDKService cfgservice = IoC.Resolve<ICauHinhDKService>();
                IYCauNghiemThuService service = IoC.Resolve<IYCauNghiemThuService>();
                ICmisProcessService cmisProcess = new CmisProcessService();
                var tiendos = cmisProcess.GetTienDo(yeucau.MaDViQLy, yeucau.MaYeuCau, yeucau.MaDDoDDien);
                tiendos = tiendos.OrderBy(p => p.KQUA_ID_BUOC).ToList();
                var listStep = cfgservice.Query.OrderBy(p => p.ORDERNUMBER).ToList();
                var tientrinhs = Query.Where(p => p.MA_YCAU_KNAI == yeucau.MaYeuCau);
                foreach (var item in tiendos)
                {
                    DvTienTrinh tientrinh = tientrinhs.FirstOrDefault(p => p.MA_CVIEC == item.MA_CVIEC);
                    if (tientrinh == null)
                        tientrinh = new DvTienTrinh();

                    tientrinh.MA_DVIQLY = item.MA_DVIQLY;
                    tientrinh.MA_YCAU_KNAI = item.MA_YCAU_KNAI;
                    tientrinh.MA_BPHAN_GIAO = item.MA_BPHAN_GIAO;
                    tientrinh.MA_NVIEN_GIAO = item.MA_NVIEN_GIAO;
                    tientrinh.MA_BPHAN_NHAN = item.MA_BPHAN_NHAN;
                    tientrinh.MA_NVIEN_NHAN = item.MA_NVIEN_NHAN;
                    tientrinh.MA_CVIEC = item.MA_CVIEC;

                    tientrinh.MA_DDO_DDIEN = item.MA_DDO_DDIEN;
                    tientrinh.MA_TNGAI = item.MA_TNGAI;
                    tientrinh.NDUNG_XLY = item.NDUNG_XLY;

                    tientrinh.NGUOI_SUA = item.NGUOI_SUA;
                    tientrinh.NGUOI_TAO = item.NGUOI_TAO;
                    tientrinh.NGUYEN_NHAN = item.NGUYEN_NHAN;
                    if (!string.IsNullOrWhiteSpace(item.SO_NGAY_LVIEC))
                        tientrinh.SO_NGAY_LVIEC = item.SO_NGAY_LVIEC;
                    tientrinh.SO_LAN = item.SO_LAN;
                    if (!string.IsNullOrWhiteSpace(item.MA_CVIECTIEP))
                        tientrinh.MA_CVIECTIEP = item.MA_CVIECTIEP;

                    var step = listStep.FirstOrDefault(p => p.MA_CVIEC_TRUOC == item.MA_CVIEC);
                    if (tientrinh.STT == 0 && step != null)
                        tientrinh.STT = step.ORDERNUMBER;

                    if (!string.IsNullOrWhiteSpace(item.NGAY_BDAU_HTHI))
                        tientrinh.NGAY_BDAU = DateTime.ParseExact(item.NGAY_BDAU_HTHI, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                    if (!string.IsNullOrWhiteSpace(item.NGAY_KTHUC_HTHI))
                        tientrinh.NGAY_KTHUC = DateTime.ParseExact(item.NGAY_KTHUC_HTHI, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

                    tientrinh.TRANG_THAI = 1;
                    Save(tientrinh);
                }

                if (tiendos.Any(p => p.MA_CVIEC == "HT" || p.MA_CVIEC == "KT"))
                {
                    if(yeucau.TrangThai < TrangThaiNghiemThu.HoanThanh)
                    {
                        yeucau.MaCViec = "HT";
                        yeucau.TrangThai = TrangThaiNghiemThu.HoanThanh;
                        service.Save(yeucau);
                    }
                }
                CommitChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public void DongBoTienTrinhHU(CongVanYeuCau yeucau)
        {
            try
            {
                ICauHinhDKService cfgservice = IoC.Resolve<ICauHinhDKService>();
                ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
                ICmisProcessService cmisProcess = new CmisProcessService();
  
                var tientrinhs = Query.Where(p => p.MA_YCAU_KNAI == yeucau.MaYeuCau);
                foreach (var item in tientrinhs)
                {
                    if (item.TRANG_THAI != 1)
                    {
                        string message = "";
                        PushToCmis(new List<DvTienTrinh>() { item }, out message);
                    }
                }

            
                CommitChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public void ThemTTrinhNT(int tthai, YCauNghiemThu yeucau, Userdata userdata)
        {
            ITThaiYeuCauService ttycausrv = IoC.Resolve<ITThaiYeuCauService>();
            var tthaiycau = ttycausrv.GetbyStatus(tthai, 1);
            if (tthaiycau != null)
            {
                var ttrinh = GetbyYCau(yeucau.MaYeuCau, tthaiycau.CVIEC_TRUOC, -1);
                if (ttrinh != null) return;

                ICauHinhDKService cauhinhsrv = IoC.Resolve<ICauHinhDKService>();
                var cviec = cauhinhsrv.GetbyMaCViec(yeucau.MaLoaiYeuCau, tthaiycau.CVIEC_TRUOC).FirstOrDefault();
                long nextstep = LastbyMaYCau(yeucau.MaYeuCau);
                if (nextstep == 0) nextstep = cviec.ORDERNUMBER;

                ttrinh = new DvTienTrinh();
                ttrinh.MA_BPHAN_GIAO = userdata.maBPhan;
                ttrinh.MA_NVIEN_GIAO = userdata.maNVien;

                ttrinh.MA_BPHAN_NHAN = userdata.maBPhan;
                ttrinh.MA_NVIEN_NHAN = userdata.maNVien;

                ttrinh.MA_CVIEC = tthaiycau.CVIEC_TRUOC;
                ttrinh.MA_CVIECTIEP = cviec.MA_CVIEC_TIEP;
                ttrinh.MA_DDO_DDIEN = yeucau.MaDDoDDien;
                ttrinh.MA_DVIQLY = yeucau.MaDViQLy;

                ttrinh.MA_YCAU_KNAI = yeucau.MaYeuCau;
                ttrinh.NDUNG_XLY = "Phân công thi công treo tháo";

                ttrinh.NGAY_BDAU = DateTime.Today;
                ttrinh.NGAY_KTHUC = DateTime.Now;
                ttrinh.NGAY_HEN = DateTime.Today;
                ttrinh.SO_LAN = 1;

                ttrinh.NGAY_TAO = DateTime.Now;
                ttrinh.NGAY_SUA = DateTime.Now;

                ttrinh.NGUOI_TAO = userdata.maNVien;
                ttrinh.NGUOI_SUA = userdata.maNVien;
                if (ttrinh.STT == 0)
                    ttrinh.STT = nextstep;

                Save(ttrinh);
                CommitChanges();
                if (ttrinh.TRANG_THAI != 1)
                {
                    string message = "";
                    PushToCmis(new List<DvTienTrinh>() { ttrinh }, out message);
                }
            }
        }

        public IList<DvTienTrinh> ListNew(string maDViQLy, string maYCau, int[] trangThais)
        {
            var query = Query.Where(p => p.MA_DVIQLY == maDViQLy && p.MA_YCAU_KNAI == maYCau);
            if (trangThais == null || trangThais.Count() == 0)
                query = query.Where(p => p.TRANG_THAI == 0);
            else
                query = query.Where(p => trangThais.Contains(p.TRANG_THAI));
            return query.OrderBy(p => p.TRANG_THAI).ToList();
        }

        public long LastbyMaYCau(string maYCau)
        {
            try
            {
                var maxstt = Query.Where(p => p.MA_YCAU_KNAI == maYCau).Max(p => p.STT);
                return maxstt + 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}