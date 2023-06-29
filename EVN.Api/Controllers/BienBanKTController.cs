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
using System.Web;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/bienbankt")]
    public class BienBanKTController : BaseController
    {
        private ILog log = LogManager.GetLogger(typeof(BienBanKTController));

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
                IBienBanKTService service = IoC.Resolve<IBienBanKTService>();
                var list = service.GetbyFilter(request.Filter.maDViQly, request.Filter.maYCau, request.Filter.keyword, request.Filter.status, fromtime, totime, pageindex, request.Paginator.pageSize, out total);

                var data = new List<BienBanKTModel>();
                foreach (var item in list)
                {
                    data.Add(new BienBanKTModel(item));
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
                IYCauNghiemThuService cvservice = IoC.Resolve<IYCauNghiemThuService>();
                IBienBanKTService service = IoC.Resolve<IBienBanKTService>();
                IKetQuaKTService kqktservice = IoC.Resolve<IKetQuaKTService>();
                IOrganizationService orgsrv = IoC.Resolve<IOrganizationService>();
                IBienBanDNService ttdnservice = IoC.Resolve<IBienBanDNService>();

                var congvan = cvservice.GetbyMaYCau(request.maYCau);

                var ttdn = ttdnservice.GetbyNo(congvan.SoThoaThuanDN, congvan.MaYeuCau);
                if (ttdn == null)
                {
                    throw new Exception("Số thỏa thuận đấu nối không hợp lệ.");
                }

                var item = service.GetbyMaYCau(congvan.MaYeuCau);

                var ketquakt = kqktservice.GetbyMaYCau(congvan.MaYeuCau);
                if (ketquakt == null)
                {
                    ketquakt = new KetQuaKT();
                    ketquakt.MA_DVIQLY = congvan.MaDViQLy;

                    ketquakt.MA_YCAU_KNAI = congvan.MaYeuCau;
                    ketquakt.MA_DDO_DDIEN = congvan.MaDDoDDien;

                    ketquakt.NGAY_HEN = DateTime.Now;
                    ketquakt.NGAY_BDAU = DateTime.Today;

                    ketquakt.MA_CVIEC_TRUOC = "KTR";
                    ketquakt.MA_LOAI_YCAU = congvan.MaLoaiYeuCau;
                    ketquakt.THUAN_LOI = true;
                }

                if (item != null && string.IsNullOrWhiteSpace(item.Data))
                {
                    item.Data = item.GetPdf();
                    service.Update(item);
                    service.CommitChanges();
                }
                if (item == null)
                {
                    var org = orgsrv.GetbyCode(congvan.MaDViQLy);
                    item = new BienBanKT();

                    item.ThoaThuanID = ttdn.ID;
                    item.SoThoaThuan = ttdn.SoBienBan;

                    item.NgayThoaThuan = ttdn.NgayLap;
                    item.MaYeuCau = congvan.MaYeuCau;
                    item.MaDViQLy = congvan.MaDViQLy;

                    item.MaSoThue = org.taxCode;
                    item.DonVi = org.orgName;

                    item.DaiDien = org.daiDien;
                    item.ChucVu = org.chucVu;

                    item.ThanhPhans = new List<ThanhPhanKT>();
                    var tpdvi = new ThanhPhanKT() { DonVi = org.orgName, Loai = 0 };
                    tpdvi.ThanhPhan = JsonConvert.SerializeObject(new List<ThanhPhanDaiDien>() { new ThanhPhanDaiDien(org.daiDien, org.chucVu) });
                    item.ThanhPhans.Add(tpdvi);

                    item.KHTen = congvan.CoQuanChuQuan;
                    item.KHDaiDien = congvan.NguoiYeuCau;

                    var tpcdt = new ThanhPhanKT() { DonVi = item.KHTen, Loai = 1 };
                    tpcdt.ThanhPhan = JsonConvert.SerializeObject(new List<ThanhPhanDaiDien>() { new ThanhPhanDaiDien(item.KHDaiDien, "") });
                    item.ThanhPhans.Add(tpcdt);

                    item.TenCongTrinh = congvan.DuAnDien;
                    item.DiaDiemXayDung = congvan.DiaChiDungDien;
                    item.ThoaThuanDauNoi = $"{congvan.DuAnDien}, địa chỉ {congvan.DiaChiDungDien}";
                    item.Data = "/bieumau/BB_KTDongDien.pdf";
                }
                item.ThuanLoi = ketquakt != null ? ketquakt.THUAN_LOI : true;
                item.MaTroNgai = ketquakt != null ? ketquakt.MA_TNGAI : String.Empty;
                if (!string.IsNullOrWhiteSpace(item.MaTroNgai))
                {
                    ITroNgaiService trongaisrv = IoC.Resolve<ITroNgaiService>();
                    var trongai = trongaisrv.Getbykey(item.MaTroNgai);
                    item.TroNgai = trongai.TEN_TNGAI;
                }

                BienBanKTData model = new BienBanKTData();
                model.KetQuaKT = new KetQuaKTModel(ketquakt);
                model.BienBanKT = new BienBanKTModel(item);
                model.BienBanKT.KetQuaKiemTra = !string.IsNullOrWhiteSpace(model.BienBanKT.KetQuaKiemTra) ? model.BienBanKT.KetQuaKiemTra : ketquakt.NDUNG_XLY;
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
        public IHttpActionResult Post([FromBody] BienBanKTModel model)
        {
            try
            {
                var userdata = this.CurrentUser;
                IBienBanKTService service = IoC.Resolve<IBienBanKTService>();
                IBienBanDNService ttdnservice = IoC.Resolve<IBienBanDNService>();
                IKetQuaKTService kqservice = IoC.Resolve<IKetQuaKTService>();

                var item = service.GetbyMaYCau(model.MaYeuCau);
                if (item == null)
                    item = new BienBanKT();
                if (item.TrangThai >= (int)TrangThaiBienBan.DaDuyet)
                {
                    log.Error("Biên bản đã được duyệt, không được sửa biên bản.");
                    return BadRequest("Biên bản đã được duyệt, không được sửa biên bản.");
                }

                item = model.ToEntity(item);
                var ttdn = ttdnservice.Getbykey(model.ThoaThuanID);
                if (ttdn == null)
                {
                    log.Error("Số thỏa thuận đấu nối không hợp lệ.");
                    throw new Exception("Số thỏa thuận đấu nối không hợp lệ.");
                }
                var ketquakt = kqservice.GetbyMaYCau(model.MaYeuCau);

                item.MaTroNgai = ketquakt.MA_TNGAI;
                if (!string.IsNullOrWhiteSpace(item.MaTroNgai))
                {
                    ITroNgaiService trongaisrv = IoC.Resolve<ITroNgaiService>();
                    var trongai = trongaisrv.Getbykey(item.MaTroNgai);
                    item.TroNgai = trongai.TEN_TNGAI;
                    ketquakt.THUAN_LOI = false;
                    item.ThuanLoi = false;
                }
                if (ketquakt.THUAN_LOI)
                {
                    ketquakt.MA_TNGAI = String.Empty;
                    ketquakt.NDUNG_XLY = String.Empty;                    
                    item.ThuanLoi = ketquakt.THUAN_LOI;
                    item.MaTroNgai = String.Empty;
                    item.TroNgai = String.Empty;
                }

                item.Data = string.Empty;
                item.NguoiLap = userdata.maNVien;
                IList<ThanhPhanKT> thanhPhanKts = new List<ThanhPhanKT>();
                IList<ThanhPhanDaiDien> thanhphanEVNs = new List<ThanhPhanDaiDien>();
                IList<ThanhPhanDaiDien> thanhphanKHs = new List<ThanhPhanDaiDien>();
                foreach (var tp in model.ThanhPhans)
                {
                    ThanhPhanDaiDien thanhPhanKT = new ThanhPhanDaiDien();
                    thanhPhanKT.ChucVu = tp.ChucVu;
                    thanhPhanKT.DaiDien = tp.DaiDien;
                    if (tp.Loai == 0)
                    {
                        thanhphanEVNs.Add(thanhPhanKT);
                    }
                    if (tp.Loai == 1)
                    {
                        thanhphanKHs.Add(thanhPhanKT);
                    }
                }
                var thanhPhanKTEVN = new ThanhPhanKT();
                var thanhPhanKTKH = new ThanhPhanKT();

                thanhPhanKTEVN.Loai = 0;
                thanhPhanKTEVN.DonVi = item.DonVi;
                thanhPhanKTEVN.ThanhPhan = JsonConvert.SerializeObject(thanhphanEVNs);

                thanhPhanKTKH.Loai = 1;
                thanhPhanKTKH.DonVi = item.DonVi;
                thanhPhanKTKH.ThanhPhan = JsonConvert.SerializeObject(thanhphanKHs);

                thanhPhanKts.Add(thanhPhanKTEVN);
                thanhPhanKts.Add(thanhPhanKTKH);
                if (service.Update(item, ttdn, thanhPhanKts) != null)
                {
                    var bienban = service.Getbykey(item.ID);
                    var ketqua = kqservice.GetbyMaYCau(item.MaYeuCau);
                    if (service.Approve(bienban, ketqua))
                        item = bienban;
                    item.ThanhPhans = thanhPhanKts;
                    model = new BienBanKTModel(item);
                    model.LapBienBan = true;
                    return Ok(model);
                }
                return BadRequest();

            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest();
            }
        }

        [JwtAuthentication]
        [HttpPut]
        public IHttpActionResult Put([FromBody] BienBanKTModel model)
        {
            try
            {
                IBienBanKTService service = IoC.Resolve<IBienBanKTService>();
                IBienBanDNService ttdnservice = IoC.Resolve<IBienBanDNService>();
                IKetQuaKTService kqservice = IoC.Resolve<IKetQuaKTService>();

                var userdata = this.CurrentUser;
                var item = service.Getbykey(model.ID);
                if (item.TrangThai >= (int)TrangThaiBienBan.DaDuyet)
                {
                    log.Error("Biên bản đã được duyệt, không được sửa biên bản.");
                    return BadRequest("Biên bản đã được duyệt, không được sửa biên bản.");
                }
                item = model.ToEntity(item);
                var ttdn = ttdnservice.Getbykey(model.ThoaThuanID);
                if (ttdn == null)
                {
                    throw new Exception("Số thỏa thuận đấu nối không hợp lệ.");
                }
                var ketquakt = kqservice.GetbyMaYCau(model.MaYeuCau);

                item.MaTroNgai = ketquakt.MA_TNGAI;
                if (!string.IsNullOrWhiteSpace(item.MaTroNgai))
                {
                    ITroNgaiService trongaisrv = IoC.Resolve<ITroNgaiService>();
                    var trongai = trongaisrv.Getbykey(item.MaTroNgai);
                    item.TroNgai = trongai.TEN_TNGAI;
                    ketquakt.THUAN_LOI = false;
                    item.ThuanLoi = false;
                }

                if (ketquakt.THUAN_LOI)
                {
                    ketquakt.MA_TNGAI = String.Empty;
                    ketquakt.NDUNG_XLY = String.Empty;
                    item.ThuanLoi = ketquakt.THUAN_LOI;
                    item.MaTroNgai = String.Empty;
                    item.TroNgai = String.Empty;
                }
                
                IList<ThanhPhanKT> thanhPhanKts = new List<ThanhPhanKT>();
                IList<ThanhPhanDaiDien> thanhphanEVNs = new List<ThanhPhanDaiDien>();
                IList<ThanhPhanDaiDien> thanhphanKHs = new List<ThanhPhanDaiDien>();
                foreach (var tp in model.ThanhPhans)
                {
                    ThanhPhanDaiDien thanhPhanKT = new ThanhPhanDaiDien();
                    thanhPhanKT.ChucVu = tp.ChucVu;
                    thanhPhanKT.DaiDien = tp.DaiDien;
                    if (tp.Loai == 0)
                    {
                        thanhphanEVNs.Add(thanhPhanKT);
                    }
                    if (tp.Loai == 1)
                    {
                        thanhphanKHs.Add(thanhPhanKT);
                    }
                }
                var thanhPhanKTEVN = new ThanhPhanKT();
                var thanhPhanKTKH = new ThanhPhanKT();

                thanhPhanKTEVN.Loai = 0;
                thanhPhanKTEVN.DonVi = item.DonVi;
                thanhPhanKTEVN.ThanhPhan = JsonConvert.SerializeObject(thanhphanEVNs);

                thanhPhanKTKH.Loai = 1;
                thanhPhanKTKH.DonVi = item.DonVi;
                thanhPhanKTKH.ThanhPhan = JsonConvert.SerializeObject(thanhphanKHs);

                thanhPhanKts.Add(thanhPhanKTEVN);
                thanhPhanKts.Add(thanhPhanKTKH);
                item.Data = string.Empty;
                if (service.Update(item, ttdn, thanhPhanKts) != null)
                {
                    var bienban = service.Getbykey(item.ID);
                    var ketqua = kqservice.GetbyMaYCau(item.MaYeuCau);
                    if (service.Approve(bienban, ketqua))
                        item = bienban;
                    item.ThanhPhans = thanhPhanKts;
                    model = new BienBanKTModel(item);
                    model.LapBienBan = true;
                    return Ok(model);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("sign")]
        public IHttpActionResult Sign([FromBody] SignModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IBienBanKTService service = IoC.Resolve<IBienBanKTService>();
                var item = service.Getbykey(model.id);
                result.success = service.Sign(item);
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
        public IHttpActionResult SignRemote([FromBody] SignModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                if (string.IsNullOrWhiteSpace(model.binary_string))
                    throw new Exception("Cần gửi biên bản kiểm tra điều kiện đóng điện điểm đấu nối đã ký số");
                IBienBanKTService service = IoC.Resolve<IBienBanKTService>();
                var item = service.Getbykey(model.id);
                var pdfdata = Convert.FromBase64String(model.binary_string);

                result.success = service.SignRemote(item, pdfdata);
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
        [Route("approve")]
        public IHttpActionResult Approve([FromBody] ApproveModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IBienBanKTService service = IoC.Resolve<IBienBanKTService>();
                IBienBanDNService ttdnservice = IoC.Resolve<IBienBanDNService>();
                IKetQuaKTService kqservice = IoC.Resolve<IKetQuaKTService>();

                var item = service.Getbykey(model.id);
                var ttdn = ttdnservice.GetbyNo(item.SoThoaThuan, item.MaYeuCau);
                if (ttdn == null)
                {
                    throw new Exception("Số thỏa thuận đấu nối không hợp lệ.");
                }
                var ketqua = kqservice.GetbyMaYCau(item.MaYeuCau);
                result.success = service.Approve(item, ketqua);
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
        [Route("detail/{id}")]
        public IHttpActionResult Detail(int id)
        {
            try
            {
                IRepository repository = new FileStoreRepository();
                IBienBanKTService service = IoC.Resolve<IBienBanKTService>();
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
        [HttpPost]
        [Route("huyketqua")]
        public IHttpActionResult HuyKetQua(CancelModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IBienBanKTService service = IoC.Resolve<IBienBanKTService>();
                IYCauNghiemThuService ycservice = IoC.Resolve<IYCauNghiemThuService>();
                IKetQuaKTService ketquasrv = IoC.Resolve<IKetQuaKTService>();

                var yeucau = ycservice.GetbyMaYCau(model.maYCau);
                var bienban = service.GetbyMaYCau(yeucau.MaYeuCau);
                var ketqua = ketquasrv.GetbyMaYCau(yeucau.MaYeuCau);
                ketqua.NGUYEN_NHAN = ketqua.NDUNG_XLY = model.noiDung;
                bienban.TroNgai = model.noiDung;
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
        [Route("cancel")]
        public IHttpActionResult Cancel([FromBody] BienBanKTModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                string message = "";
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                IBienBanKTService service = IoC.Resolve<IBienBanKTService>();

                var congvan = congvansrv.GetbyMaYCau(model.MaYeuCau);
                BienBanKT item = service.Getbykey(model.ID);
                if (congvan.TrangThai > TrangThaiNghiemThu.DuThaoHD)
                {
                    log.Error(congvan.TrangThai);
                    result.message = "Không được huỷ biên bản, yêu cầu đã dến bước thi công";
                    result.success = false;
                    return Ok(result);
                }
                if (!service.KiemTraLai(item, out message))
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
