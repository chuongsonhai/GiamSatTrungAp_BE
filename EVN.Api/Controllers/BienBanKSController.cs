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
using System.Web;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/bienbanks")]
    public class BienBanKSController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(BienBanKSController));

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
                IBienBanKSService service = IoC.Resolve<IBienBanKSService>();
                var list = service.GetbyFilter(request.Filter.maDViQly, request.Filter.maYCau, request.Filter.keyword, request.Filter.status, fromtime, totime, pageindex, request.Paginator.pageSize, out total);

                var data = new List<BienBanKSModel>();
                foreach (var item in list)
                {
                    data.Add(new BienBanKSModel(item));
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
            BienBanResult result = new BienBanResult();
            try
            {
                IBienBanKSService service = IoC.Resolve<IBienBanKSService>();
                IKetQuaKSService ketquasrv = IoC.Resolve<IKetQuaKSService>();
                ICongVanYeuCauService congvansrv = IoC.Resolve<ICongVanYeuCauService>();
                IOrganizationService orgsrv = IoC.Resolve<IOrganizationService>();

                var yeucau = congvansrv.GetbyMaYCau(request.maYCau);
                var item = service.GetbyYeuCau(yeucau.MaYeuCau);
                var ketquaks = ketquasrv.GetbyMaYCau(yeucau.MaYeuCau);
                if (ketquaks == null)
                {
                    ketquaks = new KetQuaKS();
                    ketquaks.MA_DVIQLY = yeucau.MaDViQLy;

                    ketquaks.MA_YCAU_KNAI = yeucau.MaYeuCau;
                    ketquaks.MA_DDO_DDIEN = yeucau.MaDDoDDien;

                    ketquaks.NGAY_HEN = DateTime.Now;
                    ketquaks.NGAY_BDAU = DateTime.Today;

                    ketquaks.MA_LOAI_YCAU = yeucau.MaLoaiYeuCau;
                    ketquaks.THUAN_LOI = true;

                    ketquaks.MA_CVIEC_TRUOC = "KS";
                }
                if (item != null && string.IsNullOrWhiteSpace(item.Data))
                {
                    item.Data = item.GetPdf();
                    service.Update(item);
                    service.CommitChanges();
                }
                if (item == null)
                {
                    var org = orgsrv.GetbyCode(yeucau.MaDViQLy);
                    item = new BienBanKS();

                    item.MaYeuCau = yeucau.MaYeuCau;
                    item.MaDViQLy = yeucau.MaDViQLy;

                    item.SoCongVan = yeucau.SoCongVan;
                    item.NgayCongVan = yeucau.NgayYeuCau;

                    item.MaKH = yeucau.MaKHang;

                    item.EVNDonVi = org.orgName;
                    item.EVNDaiDien = org.daiDien;
                    item.EVNChucDanh = org.chucVu;

                    item.KHTen = !string.IsNullOrWhiteSpace(yeucau.CoQuanChuQuan) ? yeucau.CoQuanChuQuan : yeucau.TenKhachHang;
                    item.KHDaiDien = yeucau.NguoiYeuCau;

                    item.ThanhPhans = new List<ThanhPhanKS>();
                    var tpdvi = new ThanhPhanKS() { DonVi = org.orgName, Loai = 0 };
                    tpdvi.ThanhPhan = JsonConvert.SerializeObject(new List<ThanhPhanDaiDien>() { new ThanhPhanDaiDien(org.daiDien, org.chucVu) });
                    item.ThanhPhans.Add(tpdvi);

                    item.KHTen = yeucau.CoQuanChuQuan;
                    item.KHDaiDien = yeucau.NguoiYeuCau;

                    var tpcdt = new ThanhPhanKS() { DonVi = item.KHTen, Loai = 1 };
                    tpcdt.ThanhPhan = JsonConvert.SerializeObject(new List<ThanhPhanDaiDien>() { new ThanhPhanDaiDien(item.KHDaiDien, item.KHChucDanh) });
                    item.ThanhPhans.Add(tpcdt);

                    item.TenCongTrinh = yeucau.DuAnDien;
                    item.DiaDiemXayDung = yeucau.DiaChiDungDien;

                    item.Data = "/bieumau/BBKhaoSat.pdf";
                }
                item.ThuanLoi = ketquaks != null ? ketquaks.THUAN_LOI : true;
                item.MaTroNgai = ketquaks != null ? ketquaks.MA_TNGAI : String.Empty;
                if (!string.IsNullOrWhiteSpace(item.MaTroNgai))
                {
                    ITroNgaiService trongaisrv = IoC.Resolve<ITroNgaiService>();
                    var trongai = trongaisrv.Getbykey(item.MaTroNgai);
                    item.TroNgai = trongai.TEN_TNGAI;
                }

                var model = new BienBanKSData();
                model.KetQuaKS = new KetQuaKSModel(ketquaks);
                model.BienBanKS = new BienBanKSModel(item);
                model.LapBienBan = ketquaks != null && ketquaks.THUAN_LOI;
                result.sign = false;
                result.success = true;
                result.data = model;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = "Lỗi lấy dữ liệu, vui lòng thực hiện lại";
                return Ok(result);
            }
        }


        [JwtAuthentication]
        [HttpPost]
        public IHttpActionResult Post([FromBody] BienBanKSModel model)
        {
            try
            {
                IBienBanKSService service = IoC.Resolve<IBienBanKSService>();
                IKetQuaKSService kqservice = IoC.Resolve<IKetQuaKSService>();
                ICongVanYeuCauService congvansrv = IoC.Resolve<ICongVanYeuCauService>();
                var yeucau = congvansrv.GetbyMaYCau(model.MaYeuCau);

                var item = service.GetbyYeuCau(yeucau.MaYeuCau);
                if (item == null) item = new BienBanKS();
                if (item.TrangThai >= (int)TrangThaiBienBan.DaDuyet)
                {
                    log.Error("Biên bản đã được duyệt, không được sửa biên bản.");
                    return BadRequest("Biên bản đã được duyệt, không được sửa biên bản.");
                }
                item = model.ToEntity(item);
                var ketquaks = kqservice.GetbyMaYCau(yeucau.MaYeuCau);

                if (!string.IsNullOrWhiteSpace(ketquaks.MA_TNGAI))
                {
                    ITroNgaiService trongaisrv = IoC.Resolve<ITroNgaiService>();
                    var trongai = trongaisrv.Getbykey(ketquaks.MA_TNGAI);
                    item.TroNgai = trongai.TEN_TNGAI;
                    ketquaks.THUAN_LOI = false;
                    item.ThuanLoi = false;

                    ICanhBaoService CBservice = IoC.Resolve<ICanhBaoService>();
                    var lcanhbao = CBservice.Query.Where(p => p.TRANGTHAI_CANHBAO <= 6);
                    var lcanhbao1 = lcanhbao.FirstOrDefault(p => p.LOAI_CANHBAO_ID == 8 && p.MA_YC == item.MaYeuCau);
                    var canhbao = new CanhBao();
                    if (lcanhbao1 == null)
                    {
                        canhbao.LOAI_CANHBAO_ID = 8;
                        canhbao.LOAI_SOLANGUI = 1;
                        canhbao.MA_YC = yeucau.MaYeuCau;
                        canhbao.THOIGIANGUI = DateTime.Now;
                        canhbao.TRANGTHAI_CANHBAO = 1;
                        canhbao.DONVI_DIENLUC = yeucau.MaDViQLy;
                        canhbao.NOIDUNG = "Loại cảnh báo 8 - lần " + canhbao.LOAI_SOLANGUI + " <br>KH: " + item.KHTen + ", SĐT: " + item.KHDienThoai + ", ĐC: " + yeucau.DiaChiCoQuan + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận: " + item.NgayLap + " Đơn vị: " + item.MaDViQLy + "<br> Khách hàng có trở ngại trong quá trình khảo sát, đơn vị kiểm tra trở ngại cập nhật trên hệ thống với thực tế tại hồ sơ và tính chất trở ngại (có thể khắc phục hoặc phải hủy yêu cầu cấp điện)";
                       
                    }
                    else
                    {
                        var checkTonTai1 = CBservice.CheckExits11(lcanhbao1.MA_YC, lcanhbao1.LOAI_CANHBAO_ID);
                        var check_tontai_mycau1 = CBservice.GetByMaYeuCautontai(lcanhbao1.MA_YC, lcanhbao1.LOAI_CANHBAO_ID);
                        TimeSpan timeDifference = DateTime.Now - check_tontai_mycau1.THOIGIANGUI;

                        if (timeDifference.TotalMinutes < 10)
                        {
                            // Nếu timeDifference nhỏ hơn 10 phút, bỏ qua và tiếp tục vòng lặp
                        }
                        else
                        {
                            if (checkTonTai1)
                            {
                                canhbao.LOAI_CANHBAO_ID = 8;
                                canhbao.LOAI_SOLANGUI = check_tontai_mycau1.LOAI_SOLANGUI + 1;
                                canhbao.MA_YC = yeucau.MaYeuCau;
                                canhbao.THOIGIANGUI = DateTime.Now;
                                canhbao.TRANGTHAI_CANHBAO = 1;
                                canhbao.DONVI_DIENLUC = yeucau.MaDViQLy;
                                canhbao.NOIDUNG = "Loại cảnh báo 8 - lần " + canhbao.LOAI_SOLANGUI + " <br>KH: " + item.KHTen + ", SĐT: " + item.KHDienThoai + ", ĐC: " + yeucau.DiaChiCoQuan + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận: " + item.NgayLap + " Đơn vị: " + item.MaDViQLy + "<br> Khách hàng có trở ngại trong quá trình khảo sát, đơn vị kiểm tra trở ngại cập nhật trên hệ thống với thực tế tại hồ sơ và tính chất trở ngại (có thể khắc phục hoặc phải hủy yêu cầu cấp điện)";

                            }
                        }
                    }

                    ILogCanhBaoService LogCBservice = IoC.Resolve<ILogCanhBaoService>();
                    string messageCB = "";
                    LogCanhBao logCB = new LogCanhBao();
                    if (CBservice.CreateCanhBao(canhbao, out messageCB))
                    {
                        logCB.CANHBAO_ID = canhbao.ID;
                        logCB.DATA_MOI = JsonConvert.SerializeObject(canhbao);
                        logCB.NGUOITHUCHIEN = HttpContext.Current.User.Identity.Name;
                        logCB.THOIGIAN = DateTime.Now;
                        logCB.TRANGTHAI = 1;
                        LogCBservice.CreateNew(logCB);
                        LogCBservice.CommitChanges();
                    }
                    else
                    {
                        throw new Exception(messageCB);
                    }
                }

                item.MaTroNgai = ketquaks.MA_TNGAI;

                item.MaYeuCau = yeucau.MaYeuCau;
                item.SoCongVan = yeucau.SoCongVan;
                item.NgayCongVan = yeucau.NgayYeuCau;
                item.MaDViQLy = yeucau.MaDViQLy;
                item.MaDViTNhan = yeucau.MaDViTNhan;
                if (ketquaks.THUAN_LOI)
                {
                    ketquaks.MA_TNGAI = String.Empty;
                    ketquaks.NDUNG_XLY = String.Empty;
                    item.ThuanLoi = ketquaks.THUAN_LOI;
                    item.MaTroNgai = String.Empty;
                    item.TroNgai = String.Empty;
                }
                IList<ThanhPhanKS> thanhPhanKSs = new List<ThanhPhanKS>();
                IList<ThanhPhanDaiDien> thanhphanEVNs = new List<ThanhPhanDaiDien>();
                IList<ThanhPhanDaiDien> thanhphanKHs = new List<ThanhPhanDaiDien>();
                foreach (var tp in model.ThanhPhans)
                {
                    ThanhPhanDaiDien thanhPhanKS = new ThanhPhanDaiDien();
                    thanhPhanKS.ChucVu = tp.ChucVu;
                    thanhPhanKS.DaiDien = tp.DaiDien;
                    if (tp.Loai == 0)
                    {
                        thanhphanEVNs.Add(thanhPhanKS);
                    }
                    if (tp.Loai == 1)
                    {
                        thanhphanKHs.Add(thanhPhanKS);
                    }
                }
                var thanhPhanKSEVN = new ThanhPhanKS();
                var thanhPhanKSKH = new ThanhPhanKS();

                thanhPhanKSEVN.Loai = 0;
                thanhPhanKSEVN.DonVi = item.EVNDonVi;
                thanhPhanKSEVN.ThanhPhan = JsonConvert.SerializeObject(thanhphanEVNs);

                thanhPhanKSKH.Loai = 1;
                thanhPhanKSKH.DonVi = item.KHTen;
                thanhPhanKSKH.ThanhPhan = JsonConvert.SerializeObject(thanhphanKHs);

                thanhPhanKSs.Add(thanhPhanKSEVN);
                thanhPhanKSs.Add(thanhPhanKSKH);

                string message = "";
                item.Data = string.Empty;                
                if (service.Update(item, thanhPhanKSs, out message) != null)
                {
                    var bienban = service.Getbykey(item.ID);
                    var ketqua = kqservice.GetbyMaYCau(bienban.MaYeuCau);
                    if (service.Approve(bienban, ketqua))
                        item = bienban;
                    item.ThanhPhans = thanhPhanKSs;
                    var bbksmodel = new BienBanKSModel(item);
                    return Ok(bbksmodel);
                }
                return BadRequest(message);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest("Có lỗi xảy ra, vui lòng thực hiện lại");
            }
        }

        [JwtAuthentication]
        [HttpPut]
        public IHttpActionResult Put([FromBody] BienBanKSModel model)
        {
            try
            {
                IBienBanKSService service = IoC.Resolve<IBienBanKSService>();
                ICongVanYeuCauService congvansrv = IoC.Resolve<ICongVanYeuCauService>();
                IKetQuaKSService kqservice = IoC.Resolve<IKetQuaKSService>();
                var yeucau = congvansrv.GetbyMaYCau(model.MaYeuCau);

                var item = service.GetbyYeuCau(model.MaYeuCau);
                if (item.TrangThai >= (int)TrangThaiBienBan.DaDuyet)
                {
                    log.Error("Biên bản đã được duyệt, không được sửa biên bản.");
                    return BadRequest("Biên bản đã được duyệt, không được sửa biên bản.");
                }
                item = model.ToEntity(item);
                var ketquaks = kqservice.GetbyMaYCau(yeucau.MaYeuCau);

                item.MaTroNgai = ketquaks.MA_TNGAI;

                item.MaYeuCau = yeucau.MaYeuCau;
                item.SoCongVan = yeucau.SoCongVan;
                item.NgayCongVan = yeucau.NgayYeuCau;
                item.MaDViQLy = yeucau.MaDViQLy;
                item.MaDViTNhan = yeucau.MaDViTNhan;

                if (!string.IsNullOrWhiteSpace(ketquaks.MA_TNGAI))
                {
                    ITroNgaiService trongaisrv = IoC.Resolve<ITroNgaiService>();
                    var trongai = trongaisrv.Getbykey(ketquaks.MA_TNGAI);
                    item.TroNgai = trongai.TEN_TNGAI;
                    ketquaks.THUAN_LOI = false;
                    item.ThuanLoi = false;
                }
                if (ketquaks.THUAN_LOI)
                {
                    ketquaks.MA_TNGAI = String.Empty;
                    ketquaks.NDUNG_XLY = String.Empty;
                    item.ThuanLoi = ketquaks.THUAN_LOI;
                    item.MaTroNgai = String.Empty;
                    item.TroNgai = String.Empty;
                }
                IList<ThanhPhanKS> thanhPhanKSs = new List<ThanhPhanKS>();
                IList<ThanhPhanDaiDien> thanhphanEVNs = new List<ThanhPhanDaiDien>();
                IList<ThanhPhanDaiDien> thanhphanKHs = new List<ThanhPhanDaiDien>();
                foreach (var tp in model.ThanhPhans)
                {
                    ThanhPhanDaiDien thanhPhanKS = new ThanhPhanDaiDien();
                    thanhPhanKS.ChucVu = tp.ChucVu;
                    thanhPhanKS.DaiDien = tp.DaiDien;
                    if (tp.Loai == 0)
                    {
                        thanhphanEVNs.Add(thanhPhanKS);
                    }
                    if (tp.Loai == 1)
                    {
                        thanhphanKHs.Add(thanhPhanKS);
                    }
                }
                var thanhPhanKSEVN = new ThanhPhanKS();
                var thanhPhanKSKH = new ThanhPhanKS();

                thanhPhanKSEVN.Loai = 0;
                thanhPhanKSEVN.DonVi = item.EVNDonVi;
                thanhPhanKSEVN.ThanhPhan = JsonConvert.SerializeObject(thanhphanEVNs);

                thanhPhanKSKH.Loai = 1;
                thanhPhanKSKH.DonVi = item.KHTen;
                thanhPhanKSKH.ThanhPhan = JsonConvert.SerializeObject(thanhphanKHs);

                thanhPhanKSs.Add(thanhPhanKSEVN);
                thanhPhanKSs.Add(thanhPhanKSKH);
                string message = "";                
                if (service.Update(item, thanhPhanKSs, out message) != null)
                {
                    var bienban = service.Getbykey(item.ID);
                    var ketqua = kqservice.GetbyMaYCau(bienban.MaYeuCau);
                    if (service.Approve(bienban, ketqua))
                        item = bienban;
                    item.ThanhPhans = thanhPhanKSs;
                    var bbksmodel = new BienBanKSModel(item);

                    return Ok(bbksmodel);
                }
                return BadRequest(message);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest("Có lỗi xảy ra, vui lòng thực hiện lại");
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("approve")]
        public IHttpActionResult Approve([FromBody] ApproveModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IBienBanKSService service = IoC.Resolve<IBienBanKSService>();
                IKetQuaKSService ketquasrv = IoC.Resolve<IKetQuaKSService>();

                var item = service.Getbykey(model.id);
                var ketquaks = ketquasrv.GetbyMaYCau(item.MaYeuCau);
                result.success = service.Approve(item, ketquaks);
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
        [HttpPost]
        [Route("sign")]
        public IHttpActionResult Sign([FromBody] ApproveModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IBienBanKSService service = IoC.Resolve<IBienBanKSService>();
                ICongViecService cvservice = IoC.Resolve<ICongViecService>();
                var congviec = cvservice.Getbykey(model.maCViec);
                var item = service.Getbykey(model.id);
                if (string.IsNullOrWhiteSpace(item.Data))
                    item.Data = item.GetPdf();

                DateTime ngayHen = DateTime.Today;
                if (!string.IsNullOrWhiteSpace(model.ngayHen))
                    ngayHen = DateTime.ParseExact(model.ngayHen, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                model.noiDung = congviec.TEN_CVIEC;

                result.success = service.Sign(item, model.maCViec, model.deptId, model.staffCode, ngayHen, model.noiDung);
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
        [HttpPost]
        [Route("signremote")]
        public IHttpActionResult SignRemote([FromBody] SignRemoteModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                if (string.IsNullOrWhiteSpace(model.binary_string))
                    throw new Exception("Cần gửi biên bản khảo sát đã ký số");
                IBienBanKSService service = IoC.Resolve<IBienBanKSService>();
                ICongViecService cvservice = IoC.Resolve<ICongViecService>();
                var congviec = cvservice.Getbykey(model.maCViec);
                var item = service.Getbykey(model.id);
                var pdfdata = Convert.FromBase64String(model.binary_string);

                DateTime ngayHen = DateTime.Today;
                if (!string.IsNullOrWhiteSpace(model.ngayHen))
                    ngayHen = DateTime.ParseExact(model.ngayHen, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                model.noiDung = congviec.TEN_CVIEC;

                result.success = service.SignRemote(item, pdfdata, model.maCViec, model.deptId, model.staffCode, ngayHen, model.noiDung);
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
        [HttpPost]
        [Route("huyketqua")]
        public IHttpActionResult HuyKetQua(CancelModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IBienBanKSService service = IoC.Resolve<IBienBanKSService>();
                IKetQuaKSService ketquasrv = IoC.Resolve<IKetQuaKSService>();

                var bienbanks = service.GetbyYeuCau(model.maYCau);
                var ketquaks = ketquasrv.GetbyMaYCau(model.maYCau);
                ketquaks.NGUYEN_NHAN = ketquaks.NDUNG_XLY = model.noiDung;
                bienbanks.TroNgai = model.noiDung;
                result.success = service.HuyHoSo(bienbanks, ketquaks);
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
        [HttpGet]
        [Route("detail/{id}")]
        public IHttpActionResult Detail(int id)
        {
            try
            {
                IRepository repository = new FileStoreRepository();
                IBienBanKSService service = IoC.Resolve<IBienBanKSService>();
                var item = service.Getbykey(id);
                if (string.IsNullOrWhiteSpace(item.Data))
                {
                    item.Data = item.GetPdf();
                    service.Save(item);
                    service.CommitChanges();
                }
                byte[] pdfdata = repository.GetData(item.Data);
                if (pdfdata == null || pdfdata.Length == 0)
                    throw new Exception();
                return Ok(pdfdata);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpGet]
        [Route("GetListTLKS/{convanid}")]
        public IHttpActionResult GetListTLKS(int convanid)
        {
            try
            {
                ITaiLieuKSService tlksservice = IoC.Resolve<ITaiLieuKSService>();
                var items = tlksservice.GetbyCongVan(convanid);
                return Ok(items);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpGet]
        [Route("GetTLKSItem/{tlksid}")]
        public IHttpActionResult GetTLKSItem(int tlksid)
        {
            try
            {
                ITaiLieuKSService tlksservice = IoC.Resolve<ITaiLieuKSService>();
                var items = tlksservice.Getbykey(tlksid);
                return Ok(items);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("CreateTLKS")]
        public IHttpActionResult CreateTLKS()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string data = null;
                data = httpRequest.Form["data"];
                ITaiLieuKSService tlksservice = IoC.Resolve<ITaiLieuKSService>();
                var item = new TaiLieuKS();
                TaiLieuKSModel model = JsonConvert.DeserializeObject<TaiLieuKSModel>(data);
                item = model.ToEntity();
                //Upload Image
                var postedFile = httpRequest.Files["File"];
                if (postedFile != null && postedFile.ContentLength > 0)
                {
                    string fileFolder = $"//TaiLieuKS//";
                    string fileName = $"{model.CongVanID}-{Guid.NewGuid().ToString("N")}{Path.GetExtension(postedFile.FileName)}";
                    string imagePath = FileUtils.SaveFilePDFAsync(postedFile, fileFolder, fileName);
                    if (string.IsNullOrEmpty(imagePath))
                    {
                        return BadRequest();
                    }
                    item.FilePath = $"/{fileFolder}/{fileName}";
                }

                tlksservice.CreateNew(item);
                tlksservice.CommitChanges();
                return Ok(item);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("UpdateTLKS")]
        public IHttpActionResult UpdateTLKS()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string data = null;
                data = httpRequest.Form["data"];
                ITaiLieuKSService tlksservice = IoC.Resolve<ITaiLieuKSService>();

                TaiLieuKSModel model = JsonConvert.DeserializeObject<TaiLieuKSModel>(data);
                var item = tlksservice.Getbykey(model.ID);
                item.Description = model.Description;
                item.Name = model.Name;
                //Upload Image
                var postedFile = httpRequest.Files["File"];
                if (postedFile != null && postedFile.ContentLength > 0)
                {
                    string fileFolder = $"//TaiLieuKS//";
                    string fileName = $"{model.CongVanID}-{Guid.NewGuid().ToString("N")}{Path.GetExtension(postedFile.FileName)}";
                    string imagePath = FileUtils.SaveFilePDFAsync(postedFile, fileFolder, fileName);
                    if (string.IsNullOrEmpty(imagePath))
                    {
                        return BadRequest();
                    }
                    item.FilePath = $"/{fileFolder}/{fileName}";
                }
                tlksservice.Update(item);
                tlksservice.CommitChanges();
                return Ok(item);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("UploadFileTLKS")]
        public IHttpActionResult UploadFileTLKS()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string data = null;
                data = httpRequest.Form["data"];
                ITaiLieuKSService tlksservice = IoC.Resolve<ITaiLieuKSService>();

                TaiLieuKSModel model = JsonConvert.DeserializeObject<TaiLieuKSModel>(data);
                var item = new TaiLieuKS();
                item = model.ToEntity();
                if (model.ID > 0)
                {
                    item = tlksservice.Getbykey(model.ID);
                }
                item.Description = model.Description;
                item.Name = model.Name;
                //Upload Image
                var postedFile = httpRequest.Files["File"];
                if (postedFile != null && postedFile.ContentLength > 0)
                {
                    string fileFolder = $"//TaiLieuKS//";
                    string fileName = $"{model.CongVanID}-{Guid.NewGuid().ToString("N")}{Path.GetExtension(postedFile.FileName)}";
                    string imagePath = FileUtils.SaveFile(postedFile, fileFolder, fileName);
                    if (string.IsNullOrEmpty(imagePath))
                    {
                        return BadRequest();
                    }
                    item.FilePath = $"/{fileFolder}/{fileName}";
                }
                tlksservice.Save(item);
                tlksservice.CommitChanges();
                return Ok(item);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest();
            }
        }

        [JwtAuthentication]
        [HttpDelete]
        [Route("delete/{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {

                ITaiLieuKSService service = IoC.Resolve<ITaiLieuKSService>();
                service.Delete(id);
                service.CommitChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("cancel")]
        public IHttpActionResult Cancel([FromBody] BienBanKSModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                string message = "";
                ICongVanYeuCauService congvansrv = IoC.Resolve<ICongVanYeuCauService>();
                IBienBanKSService service = IoC.Resolve<IBienBanKSService>();

                var congvan = congvansrv.GetbyMaYCau(model.MaYeuCau);
                BienBanKS item = service.Getbykey(model.ID);
                if (congvan.TrangThai > TrangThaiCongVan.BienBanKS)
                {
                    log.Error(congvan.TrangThai);
                    result.message = "Không được huỷ biên bản, yêu cầu đã dến bước dự thảo thoả thuận";
                    result.success = false;
                    return Ok(result);
                }
                if (!service.KhaoSatLai(item, out message))
                {
                    result.message = "Có lỗi xảy ra, vui lòng thực hiện lại";
                    result.success = false;
                    return Ok(result);
                }
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại";
                result.success = false;
                return Ok(result);
            }
        }
    }
}
