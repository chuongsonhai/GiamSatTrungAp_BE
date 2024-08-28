using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Api.Model.Request;
using EVN.Core;
using EVN.Core.CMIS;
using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Repository;
using EVN.Core.Utilities;
using FX.Core;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/bienbantt")]
    public class BienBanTTController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(BienBanTTController));

        [JwtAuthentication]
        [HttpPost]
        [Route("filter")]
        public IHttpActionResult Filter(BienBanRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                int total = 0;
                DateTime fromtime = DateTime.ParseExact(request.Filter.fromdate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                DateTime totime = DateTime.ParseExact(request.Filter.todate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                IBienBanTTService service = IoC.Resolve<IBienBanTTService>();
                var list = service.GetbyFilter(request.Filter.maDViQly, request.Filter.maYCau, request.Filter.keyword, request.Filter.status, fromtime, totime, pageindex, request.Paginator.pageSize, out total);

                var data = new List<BienBanTTModel>();
                foreach (var item in list)
                {
                    data.Add(new BienBanTTModel(item));
                }
                result.total = total;
                result.data = data;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                return Ok(result);
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("getdata")]
        public IHttpActionResult GetData(TTYeuCauRequest request)
        {
            try
            {
                IBienBanTTService service = IoC.Resolve<IBienBanTTService>();
                IKetQuaTCService kqservice = IoC.Resolve<IKetQuaTCService>();
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                IOrganizationService orgsrv = IoC.Resolve<IOrganizationService>();
                IBienBanDNService ttdnservice = IoC.Resolve<IBienBanDNService>();

                var yeucau = congvansrv.GetbyMaYCau(request.maYCau);
                var item = service.GetbyMaYCau(yeucau.MaYeuCau);
                var ttdn = ttdnservice.GetbyNo(yeucau.SoThoaThuanDN, yeucau.MaYeuCau);
                if (string.IsNullOrWhiteSpace(yeucau.MaTram) || string.IsNullOrWhiteSpace(yeucau.MaKHang))
                    yeucau = congvansrv.SyncData(yeucau);

                var ketquatc = kqservice.GetbyMaYCau(yeucau.MaYeuCau);
                if (ketquatc == null)
                {
                    ketquatc = new KetQuaTC();
                    ketquatc.MA_DVIQLY = yeucau.MaDViQLy;

                    ketquatc.MA_YCAU_KNAI = yeucau.MaYeuCau;
                    ketquatc.MA_DDO_DDIEN = yeucau.MaDDoDDien;

                    ketquatc.NGAY_BDAU = DateTime.Today;
                    ketquatc.NGAY_HEN = DateTime.Now;

                    ketquatc.MA_LOAI_YCAU = yeucau.MaLoaiYeuCau;
                    ketquatc.MA_CVIEC_TRUOC = "TC";
                    ketquatc.THUAN_LOI = true;
                }
                if (item == null)
                {
                    var org = orgsrv.GetbyCode(yeucau.MaDViQLy);
                    item = new BienBanTT();

                    item.MA_DVIQLY = ttdn.MaDViQLy;
                    item.MA_YCAU_KNAI = ttdn.MaYeuCau;

                    item.MA_DDO = yeucau.MaKHang;
                    item.TEN_KHACHHANG = ttdn.KHTen;
                    item.TEN_CTY = org.orgName;
                    item.SDT_KHACHHANG = yeucau.DienThoai;

                    item.MA_TRAM = yeucau.MaTram;

                    item.NGUOI_DDIEN = ttdn.KHDaiDien;

                    item.TEN_DLUC = org.orgName;

                    item.DIA_DIEM = yeucau.DiaChiDungDien;
                    item.VTRI_LDAT = yeucau.DiaChiDungDien;
                }

                BienBanTTData model = new BienBanTTData();
                model.BienBanTT = new BienBanTTModel(item);
                model.KetQuaTC = new KetQuaTCModel(ketquatc);
                model.LapBienBan = ketquatc != null && ketquatc.TRANG_THAI == 1;
                return Ok(model);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            try
            {
                IBienBanTTService service = IoC.Resolve<IBienBanTTService>();
                IKetQuaTCService kqservice = IoC.Resolve<IKetQuaTCService>();
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                IOrganizationService orgsrv = IoC.Resolve<IOrganizationService>();
                IBienBanDNService ttdnservice = IoC.Resolve<IBienBanDNService>();

                var congvan = congvansrv.Getbykey(id);
                var item = service.GetbyMaYCau(congvan.MaYeuCau);
                var ttdn = ttdnservice.GetbyNo(congvan.SoThoaThuanDN, congvan.MaYeuCau);
                if (string.IsNullOrWhiteSpace(congvan.MaTram) || string.IsNullOrWhiteSpace(congvan.MaKHang))
                    congvan = congvansrv.SyncData(congvan);

                var ketquatc = kqservice.GetbyMaYCau(congvan.MaYeuCau);
                if (ketquatc == null)
                {
                    ketquatc = new KetQuaTC();
                    ketquatc.MA_DVIQLY = congvan.MaDViQLy;

                    ketquatc.MA_YCAU_KNAI = congvan.MaYeuCau;
                    ketquatc.MA_DDO_DDIEN = congvan.MaDDoDDien;

                    ketquatc.NGAY_BDAU = DateTime.Today;
                    ketquatc.NGAY_HEN = DateTime.Now;

                    ketquatc.MA_LOAI_YCAU = congvan.MaLoaiYeuCau;
                    ketquatc.MA_CVIEC_TRUOC = "TC";
                    ketquatc.THUAN_LOI = true;
                }
                if (item == null)
                {
                    var org = orgsrv.GetbyCode(congvan.MaDViQLy);
                    item = new BienBanTT();

                    item.MA_DVIQLY = ttdn.MaDViQLy;
                    item.MA_YCAU_KNAI = ttdn.MaYeuCau;

                    item.MA_DDO = congvan.MaKHang;
                    item.TEN_KHACHHANG = ttdn.KHTen;
                    item.TEN_CTY = org.orgName;
                    item.SDT_KHACHHANG = congvan.DienThoai;

                    item.MA_TRAM = congvan.MaTram;

                    item.NGUOI_DDIEN = ttdn.KHDaiDien;

                    item.TEN_DLUC = org.orgName;

                    item.DIA_DIEM = congvan.DiaChiDungDien;
                    item.VTRI_LDAT = congvan.DiaChiDungDien;

                    service.CreateNew(item);
                    service.CommitChanges();
                }

                BienBanTTData model = new BienBanTTData();
                model.BienBanTT = new BienBanTTModel(item);
                model.KetQuaTC = new KetQuaTCModel(ketquatc);
                model.LapBienBan = ketquatc != null && ketquatc.TRANG_THAI == 1;
                return Ok(model);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("Create")]
        public IHttpActionResult Create(BienBanTTModel model)
        {
            try
            {

                if (model == null) throw new Exception("Dữ liệu không hợp lệ");
                IBienBanTTService service = IoC.Resolve<IBienBanTTService>();
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                var congvan = congvansrv.GetbyMaYCau(model.MA_YCAU_KNAI);
                model.MA_DVIQLY = congvan.MaDViQLy;

                var bbtt = new BienBanTT();
                bbtt = model.ToEntity(bbtt);

                var congtoTreo = new CongTo();
                if (model.ChiTietThietBiTreo.IsCongTo)
                {
                    congtoTreo = model.ChiTietThietBiTreo.CongTo.ToEntity(congtoTreo);
                }

                var lstMBDTreo = new List<MayBienDong>();
                if (model.ChiTietThietBiTreo.IsMBD)
                {
                    foreach (var item in model.ChiTietThietBiTreo.MayBienDongs)
                    {
                        MayBienDong mayBienDong = new MayBienDong();
                        mayBienDong = item.ToEntity(mayBienDong, bbtt.MA_DVIQLY);
                        log.ErrorFormat("Ma chi so hop vien ti: {0}", mayBienDong.CHIHOP_VIEN);
                        log.ErrorFormat("Ma tem so vien ti: {0}", mayBienDong.TEM_KD_VIEN);
                        mayBienDong.TI_THAO = false;
                        lstMBDTreo.Add(mayBienDong);
                    }
                }
                var lstMBDATreo = new List<MayBienDienAp>();
                if (model.ChiTietThietBiTreo.IsMBDA)
                {
                    foreach (var item in model.ChiTietThietBiTreo.MayBienDienAps)
                    {
                        MayBienDienAp mayBienDienAp = new MayBienDienAp();
                        mayBienDienAp = item.ToEntity(mayBienDienAp);
                        log.ErrorFormat("Ma chi so hop vien tu: {0}", mayBienDienAp.CHIHOP_VIEN);
                        log.ErrorFormat("Ma tem so vien tu: {0}", mayBienDienAp.TEM_KD_VIEN);
                        mayBienDienAp.TU_THAO = false;
                        lstMBDATreo.Add(mayBienDienAp);
                    }
                }

                var congtoThao = new CongTo();
                congtoThao = model.ChiTietThietBiThao.CongTo.ToEntity(congtoThao);
                var lstMBDThao = new List<MayBienDong>();
                foreach (var item in model.ChiTietThietBiThao.MayBienDongs)
                {
                    MayBienDong mayBienDong = new MayBienDong();
                    mayBienDong = item.ToEntity(mayBienDong, bbtt.MA_DVIQLY);
                    mayBienDong.TI_THAO = true;
                    lstMBDThao.Add(mayBienDong);
                }
                var lstMBDAThao = new List<MayBienDienAp>();
                foreach (var item in model.ChiTietThietBiThao.MayBienDienAps)
                {
                    MayBienDienAp mayBienDienAp = new MayBienDienAp();
                    mayBienDienAp = item.ToEntity(mayBienDienAp);
                    mayBienDienAp.TU_THAO = true;
                    lstMBDAThao.Add(mayBienDienAp);
                }

                var lstCongTo = new List<CongTo>();
                lstCongTo.Add(congtoThao);
                lstCongTo.Add(congtoTreo);

                var lstMBD = new List<MayBienDong>();
                lstMBD.AddRange(lstMBDTreo);
                lstMBD.AddRange(lstMBDThao);

                var lstMBDA = new List<MayBienDienAp>();
                lstMBDA.AddRange(lstMBDATreo);
                lstMBDA.AddRange(lstMBDAThao);
                log.ErrorFormat("Don vi hien thi: {0}", congtoTreo.DONVI_HIENTHI);
                log.ErrorFormat("He thong do dem: {0}", congtoTreo.HSO_HTDODEM);
                if (service.CreateNew(bbtt, lstCongTo, lstMBD, lstMBDA) != null)
                {
                    IKetQuaTCService kqservice = IoC.Resolve<IKetQuaTCService>();
                    var bienbantt = service.GetbyMaYCau(model.MA_YCAU_KNAI);
                    var yeucau = congvansrv.GetbyMaYCau(bienbantt.MA_YCAU_KNAI);
                    var ketqua = kqservice.GetbyMaYCau(bienbantt.MA_YCAU_KNAI);
                    if (!service.Approve(yeucau, bienbantt, ketqua))
                        return BadRequest();
                    bienbantt.CongTos = lstCongTo;
                    bienbantt.MayBienDongs = lstMBD;
                    bienbantt.MayBienDienAps = lstMBDA;
                    return Ok(new BienBanTTModel(bienbantt));
                }
                return Ok(new BienBanTTModel(bbtt));
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("Update")]
        public IHttpActionResult Update(BienBanTTModel model)
        {
            try
            {

                if (model == null) throw new Exception("Dữ liệu không hợp lệ");
                IBienBanTTService service = IoC.Resolve<IBienBanTTService>();
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                var congvan = congvansrv.GetbyMaYCau(model.MA_YCAU_KNAI);
                model.MA_DVIQLY = congvan.MaDViQLy;

                var bbtt = service.Getbykey(model.ID);
                bbtt = model.ToEntity(bbtt);

                var congtoTreo = new CongTo();
                if (model.ChiTietThietBiTreo.IsCongTo)
                {
                    congtoTreo = model.ChiTietThietBiTreo.CongTo.ToEntity(congtoTreo);
                }

                var lstMBDTreo = new List<MayBienDong>();
                if (model.ChiTietThietBiTreo.IsMBD)
                {
                    foreach (var item in model.ChiTietThietBiTreo.MayBienDongs)
                    {
                        MayBienDong mayBienDong = new MayBienDong();
                        mayBienDong = item.ToEntity(mayBienDong, bbtt.MA_DVIQLY);
                        mayBienDong.TI_THAO = false;
                        lstMBDTreo.Add(mayBienDong);
                    }
                }
                var lstMBDATreo = new List<MayBienDienAp>();
                if (model.ChiTietThietBiTreo.IsMBDA)
                {
                    foreach (var item in model.ChiTietThietBiTreo.MayBienDienAps)
                    {
                        MayBienDienAp mayBienDienAp = new MayBienDienAp();
                        mayBienDienAp = item.ToEntity(mayBienDienAp);
                        mayBienDienAp.TU_THAO = false;
                        lstMBDATreo.Add(mayBienDienAp);
                    }
                }

                var congtoThao = new CongTo();
                congtoThao = model.ChiTietThietBiThao.CongTo.ToEntity(congtoThao);
                var lstMBDThao = new List<MayBienDong>();
                foreach (var item in model.ChiTietThietBiThao.MayBienDongs)
                {
                    MayBienDong mayBienDong = new MayBienDong();
                    mayBienDong = item.ToEntity(mayBienDong, bbtt.MA_DVIQLY);
                    mayBienDong.TI_THAO = true;
                    lstMBDThao.Add(mayBienDong);
                }
                var lstMBDAThao = new List<MayBienDienAp>();
                foreach (var item in model.ChiTietThietBiThao.MayBienDienAps)
                {
                    MayBienDienAp mayBienDienAp = new MayBienDienAp();
                    mayBienDienAp = item.ToEntity(mayBienDienAp);
                    mayBienDienAp.TU_THAO = true;
                    lstMBDAThao.Add(mayBienDienAp);
                }

                var lstCongTo = new List<CongTo>();
                lstCongTo.Add(congtoThao);
                lstCongTo.Add(congtoTreo);

                var lstMBD = new List<MayBienDong>();
                lstMBD.AddRange(lstMBDTreo);
                lstMBD.AddRange(lstMBDThao);

                var lstMBDA = new List<MayBienDienAp>();
                lstMBDA.AddRange(lstMBDATreo);
                lstMBDA.AddRange(lstMBDAThao);

                if (service.Update(bbtt, lstCongTo, lstMBD, lstMBDA) != null)
                {
                    IKetQuaTCService kqservice = IoC.Resolve<IKetQuaTCService>();
                    var bienbantt = service.GetbyMaYCau(model.MA_YCAU_KNAI);
                    var yeucau = congvansrv.GetbyMaYCau(bienbantt.MA_YCAU_KNAI);
                    var ketqua = kqservice.GetbyMaYCau(bienbantt.MA_YCAU_KNAI);
                    if (!service.Approve(yeucau, bienbantt, ketqua))
                        return BadRequest();
                    bienbantt.CongTos = lstCongTo;
                    bienbantt.MayBienDongs = lstMBD;
                    bienbantt.MayBienDienAps = lstMBDA;
                    return Ok(new BienBanTTModel(bienbantt));
                }
                return Ok(new BienBanTTModel(bbtt));
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("CreateBBTT")]
        public IHttpActionResult CreateBBTT()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string data = null;
                data = httpRequest.Form["data"];
                BienBanTTModel model = JsonConvert.DeserializeObject<BienBanTTModel>(data);
                if (model == null) throw new Exception("Dữ liệu không hợp lệ");
                IBienBanTTService service = IoC.Resolve<IBienBanTTService>();
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                var congvan = congvansrv.GetbyMaYCau(model.MA_YCAU_KNAI);
                model.MA_DVIQLY = congvan.MaDViQLy;

                var bbtt = new BienBanTT();
                bbtt = model.ToEntity(bbtt);

                var congtoTreo = new CongTo();
                if (model.ChiTietThietBiTreo.IsCongTo)
                {
                    congtoTreo = model.ChiTietThietBiTreo.CongTo.ToEntity(congtoTreo);
                }

                var lstMBDTreo = new List<MayBienDong>();
                if (model.ChiTietThietBiTreo.IsMBD)
                {
                    foreach (var item in model.ChiTietThietBiTreo.MayBienDongs)
                    {
                        MayBienDong mayBienDong = new MayBienDong();
                        mayBienDong = item.ToEntity(mayBienDong, bbtt.MA_DVIQLY);
                        mayBienDong.TI_THAO = false;
                        lstMBDTreo.Add(mayBienDong);
                    }
                }
                var lstMBDATreo = new List<MayBienDienAp>();
                if (model.ChiTietThietBiTreo.IsMBDA)
                {
                    foreach (var item in model.ChiTietThietBiTreo.MayBienDienAps)
                    {
                        MayBienDienAp mayBienDienAp = new MayBienDienAp();
                        mayBienDienAp = item.ToEntity(mayBienDienAp);
                        mayBienDienAp.TU_THAO = false;
                        lstMBDATreo.Add(mayBienDienAp);
                    }
                }

                var congtoThao = new CongTo();
                congtoThao = model.ChiTietThietBiThao.CongTo.ToEntity(congtoThao);
                var lstMBDThao = new List<MayBienDong>();
                foreach (var item in model.ChiTietThietBiThao.MayBienDongs)
                {
                    MayBienDong mayBienDong = new MayBienDong();
                    mayBienDong = item.ToEntity(mayBienDong, bbtt.MA_DVIQLY);
                    mayBienDong.TI_THAO = true;
                    lstMBDThao.Add(mayBienDong);
                }
                var lstMBDAThao = new List<MayBienDienAp>();
                foreach (var item in model.ChiTietThietBiThao.MayBienDienAps)
                {
                    MayBienDienAp mayBienDienAp = new MayBienDienAp();
                    mayBienDienAp = item.ToEntity(mayBienDienAp);
                    mayBienDienAp.TU_THAO = true;
                    lstMBDAThao.Add(mayBienDienAp);
                }

                var lstCongTo = new List<CongTo>();
                lstCongTo.Add(congtoThao);
                lstCongTo.Add(congtoTreo);

                var lstMBD = new List<MayBienDong>();
                lstMBD.AddRange(lstMBDTreo);
                lstMBD.AddRange(lstMBDThao);

                var lstMBDA = new List<MayBienDienAp>();
                lstMBDA.AddRange(lstMBDATreo);
                lstMBDA.AddRange(lstMBDAThao);

                //Upload Image
                var postedFile = httpRequest.Files["File"];
                if (postedFile != null && postedFile.ContentLength > 0)
                {
                    string fileFolder = $"BienBanTreoThao";
                    string fileName = $"{congvan.SoCongVan}-{Guid.NewGuid().ToString("N")}{Path.GetExtension(postedFile.FileName)}";
                    string imagePath = FileUtils.SaveFilePDFAsync(postedFile, fileFolder, fileName);
                    if (string.IsNullOrEmpty(imagePath))
                    {
                        return BadRequest();
                    }
                    bbtt.Data = $"/{fileFolder}/{fileName}";
                }
                bbtt = service.CreateNew(bbtt, lstCongTo, lstMBD, lstMBDA);
                return Ok(new BienBanTTModel(bbtt));
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("UpdateBBTT")]
        public IHttpActionResult UpdateBBTT()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string data = null;
                data = httpRequest.Form["data"];
                BienBanTTModel model = JsonConvert.DeserializeObject<BienBanTTModel>(data);
                if (model == null) throw new Exception("Dữ liệu không hợp lệ");
                IBienBanTTService service = IoC.Resolve<IBienBanTTService>();
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                var congvan = congvansrv.GetbyMaYCau(model.MA_YCAU_KNAI);
                model.MA_DVIQLY = congvan.MaDViQLy;

                var bbtt = service.Getbykey(model.ID);
                bbtt = model.ToEntity(bbtt);

                var congtoTreo = new CongTo();
                if (model.ChiTietThietBiTreo.IsCongTo)
                {
                    congtoTreo = model.ChiTietThietBiTreo.CongTo.ToEntity(congtoTreo);
                }

                var lstMBDTreo = new List<MayBienDong>();
                if (model.ChiTietThietBiTreo.IsMBD)
                {
                    foreach (var item in model.ChiTietThietBiTreo.MayBienDongs)
                    {
                        MayBienDong mayBienDong = new MayBienDong();
                        mayBienDong = item.ToEntity(mayBienDong, bbtt.MA_DVIQLY);
                        mayBienDong.TI_THAO = false;
                        lstMBDTreo.Add(mayBienDong);
                    }
                }
                var lstMBDATreo = new List<MayBienDienAp>();
                if (model.ChiTietThietBiTreo.IsMBDA)
                {
                    foreach (var item in model.ChiTietThietBiTreo.MayBienDienAps)
                    {
                        MayBienDienAp mayBienDienAp = new MayBienDienAp();
                        mayBienDienAp = item.ToEntity(mayBienDienAp);
                        mayBienDienAp.TU_THAO = false;
                        lstMBDATreo.Add(mayBienDienAp);
                    }
                }

                var congtoThao = new CongTo();
                congtoThao = model.ChiTietThietBiThao.CongTo.ToEntity(congtoThao);
                var lstMBDThao = new List<MayBienDong>();
                foreach (var item in model.ChiTietThietBiThao.MayBienDongs)
                {
                    MayBienDong mayBienDong = new MayBienDong();
                    mayBienDong = item.ToEntity(mayBienDong, bbtt.MA_DVIQLY);
                    mayBienDong.TI_THAO = true;
                    lstMBDThao.Add(mayBienDong);
                }
                var lstMBDAThao = new List<MayBienDienAp>();
                foreach (var item in model.ChiTietThietBiThao.MayBienDienAps)
                {
                    MayBienDienAp mayBienDienAp = new MayBienDienAp();
                    mayBienDienAp = item.ToEntity(mayBienDienAp);
                    mayBienDienAp.TU_THAO = true;
                    lstMBDAThao.Add(mayBienDienAp);
                }

                var lstCongTo = new List<CongTo>();
                lstCongTo.Add(congtoThao);
                lstCongTo.Add(congtoTreo);

                var lstMBD = new List<MayBienDong>();
                lstMBD.AddRange(lstMBDTreo);
                lstMBD.AddRange(lstMBDThao);

                var lstMBDA = new List<MayBienDienAp>();
                lstMBDA.AddRange(lstMBDATreo);
                lstMBDA.AddRange(lstMBDAThao);

                //Upload Image
                var postedFile = httpRequest.Files["File"];
                if (postedFile != null && postedFile.ContentLength > 0)
                {
                    string fileFolder = $"BienBanTreoThao";
                    string fileName = $"{congvan.SoCongVan}-{Guid.NewGuid().ToString("N")}{Path.GetExtension(postedFile.FileName)}";
                    string imagePath = FileUtils.SaveFilePDFAsync(postedFile, fileFolder, fileName);
                    if (string.IsNullOrEmpty(imagePath))
                    {
                        return BadRequest();
                    }
                    bbtt.Data = $"/{fileFolder}/{fileName}";
                }

                bbtt = service.Update(bbtt, lstCongTo, lstMBD, lstMBDA);
                return Ok(new BienBanTTModel(bbtt));
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("approve")]
        public IHttpActionResult Approve(ApproveModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IBienBanTTService service = IoC.Resolve<IBienBanTTService>();
                IKetQuaTCService kqservice = IoC.Resolve<IKetQuaTCService>();
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                var item = service.Getbykey(model.id);
                var yeucau = congvansrv.GetbyMaYCau(item.MA_YCAU_KNAI);
                var ketqua = kqservice.GetbyMaYCau(item.MA_YCAU_KNAI);
                result.success = service.Approve(yeucau, item, ketqua);
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = ex.Message;
                return Ok(result);
            }
        }

        [JwtAuthentication]
        [HttpGet]
        [Route("getpdf/{id}")]
        public IHttpActionResult GetPdf(int id)
        {
            try
            {
                IBienBanTTService service = IoC.Resolve<IBienBanTTService>();
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                IRepository repository = new FileStoreRepository();
                var yeucau = congvansrv.Getbykey(id);
                var item = service.GetbyMaYCau(yeucau.MaYeuCau);                
                string maDViQly = yeucau.MaDViQLy;
                string maYCau = yeucau.MaYeuCau;
                string maLoaiHSo = LoaiHSoCode.BB_TT;
                ICmisProcessService cmisProcess = new CmisProcessService();
                byte[] pdfdata = cmisProcess.GetData(maDViQly, maYCau, maLoaiHSo);
                if (pdfdata != null && pdfdata.Length > 0)
                {
                    service.UpdatebyCMIS(item, yeucau, pdfdata);
                    return Ok(pdfdata);
                }
                return Ok(repository.GetData("/bieumau/BBTreoThao.pdf"));
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpGet]
        [Route("DetailPdf/{maYC}")]
        public IHttpActionResult DetailPdf(string maYC)
        {
            try
            {
                IBienBanTTService service = IoC.Resolve<IBienBanTTService>();
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                IRepository repository = new FileStoreRepository();
                var yeucau = congvansrv.GetbyMaYCau(maYC);
                var item = service.GetbyMaYCau(yeucau.MaYeuCau);
                if (item.TRANG_THAI >= (int)TrangThaiBienBan.HoanThanh)
                {
                    return Ok(repository.GetData(item.Data));
                }

                string maDViQly = yeucau.MaDViQLy;
                string maYCau = yeucau.MaYeuCau;
                string maLoaiHSo = LoaiHSoCode.BB_TT;
                ICmisProcessService cmisProcess = new CmisProcessService();
                byte[] pdfdata = cmisProcess.GetData(maDViQly, maYCau, maLoaiHSo);
                if (pdfdata != null && pdfdata.Length > 0)
                {
                    service.UpdatebyCMIS(item, yeucau, pdfdata);
                    return Ok(pdfdata);
                }
                return Ok(repository.GetData("/bieumau/BBTreoThao.pdf"));
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpGet]
        [Route("detail/{id}")]
        public IHttpActionResult Detail(int id)
        {
            try
            {
                IBienBanTTService service = IoC.Resolve<IBienBanTTService>();
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                IRepository repository = new FileStoreRepository();
                
                var bbantt = service.Getbykey(id);
                var yeucau = congvansrv.GetbyMaYCau(bbantt.MA_YCAU_KNAI);
                var item = service.GetbyMaYCau(yeucau.MaYeuCau);
                if (item.TRANG_THAI >= (int)TrangThaiBienBan.HoanThanh)
                {
                    return Ok(repository.GetData(item.Data));
                }

                string maDViQly = yeucau.MaDViQLy;
                string maYCau = yeucau.MaYeuCau;
                string maLoaiHSo = LoaiHSoCode.BB_TT;
                ICmisProcessService cmisProcess = new CmisProcessService();
                byte[] pdfdata = cmisProcess.GetData(maDViQly, maYCau, maLoaiHSo);
                if (pdfdata != null && pdfdata.Length > 0)
                {
                    service.UpdatebyCMIS(item, yeucau, pdfdata);
                    return Ok(pdfdata);
                }
                return Ok(repository.GetData("/bieumau/BBTreoThao.pdf"));
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("huyketqua")]
        public IHttpActionResult HuyKetQua(CancelModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IBienBanTTService service = IoC.Resolve<IBienBanTTService>();
                IYCauNghiemThuService ycservice = IoC.Resolve<IYCauNghiemThuService>();
                IKetQuaTCService ketquasrv = IoC.Resolve<IKetQuaTCService>();

                var yeucau = ycservice.GetbyMaYCau(model.maYCau);
                var bienban = service.GetbyMaYCau(yeucau.MaYeuCau);
                var ketqua = ketquasrv.GetbyMaYCau(yeucau.MaYeuCau);
                ketqua.NGUYEN_NHAN = model.noiDung;
                result.success = service.HuyKetQua(bienban, ketqua);
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                return Ok(result);
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("signnvien")]
        public IHttpActionResult SignNVien(SignBienBanTT model)
        {
            ResponseResult result = new ResponseResult();
            IRepository repository = new FileStoreRepository();
            IBienBanTTService service = IoC.Resolve<IBienBanTTService>();
            IHoSoGiayToService hsoservice = IoC.Resolve<IHoSoGiayToService>();

            try
            {
                var item = service.Getbykey(model.id);
                string maLoaiHSo = LoaiHSoCode.BB_TT;
                ICmisProcessService cmisProcess = new CmisProcessService();
                if (!item.KyNVTT || !item.KyNVNP)
                {
                    string doiTuong = model.SignTT ? "NVTT" : "NVNP";
                    string maNVien = model.SignTT ? item.NVIEN_TTHAO : item.NVIEN_NPHONG;
                    if (string.IsNullOrWhiteSpace(maNVien))
                    {
                        result.success = false;
                        result.message = "Chưa có thông tin mã nhân viên";
                        return Ok(result);
                    }
                    var signed = cmisProcess.SignNhanVien(item.MA_DVIQLY, item.MA_YCAU_KNAI, maNVien, maLoaiHSo, doiTuong);
                    if (!signed)
                    {
                        result.success = false;
                        result.message = "Có lỗi xảy ra, vui lòng thực hiện lại";
                        return Ok(result);
                    }
                    

                    byte[] pdfdata = cmisProcess.GetData(item.MA_DVIQLY, item.MA_YCAU_KNAI, maLoaiHSo);
                    if (pdfdata != null && pdfdata.Length > 0)
                    {
                        service.BeginTran();
                        string folder = $"{item.MA_DVIQLY}/{item.MA_YCAU_KNAI}";
                        item.KyNVTT = model.SignTT;
                        item.KyNVNP = !model.SignTT;
                        item.Data = repository.Store(folder, pdfdata, item.Data);                        
                        var hoso = hsoservice.Get(p => p.MaDViQLy == item.MA_DVIQLY && p.MaYeuCau == item.MA_YCAU_KNAI && p.LoaiHoSo == maLoaiHSo);
                        if (hoso == null)
                        {
                            hoso = new HoSoGiayTo();
                            hoso.MaHoSo = Guid.NewGuid().ToString("N");
                            hoso.TenHoSo = "Biên bản treo tháo";
                            hoso.LoaiHoSo = maLoaiHSo;
                        }
                        hoso.Data = item.Data;
                        hsoservice.Save(hoso);
                        service.Save(item);
                        service.CommitTran();
                        result.success = true;
                        return Ok(result);
                    }
                    result.success = false;
                    result.message = "Có lỗi xảy ra, vui lòng kiểm tra lại kết nối";
                    return Ok(result);
                }
                result.success = true;
                result.message = "Biên bản treo tháo đã đủ chữ ký";
                return Ok(result);                
            }
            catch (Exception ex)
            {
                service.RolbackTran();
                log.Error(ex);
                result.success = false;
                result.message = ex.Message;
                return Ok(result);
            }
        }
    }
}
