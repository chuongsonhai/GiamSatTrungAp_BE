using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Api.Model.Request;
using EVN.Core;
using EVN.Core.CMIS;
using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Repository;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace EVN.Api.Controllers
{

    [RoutePrefix("api/bienbannt")]
    public class BienBanNTController : BaseController
    {
        private ILog log = LogManager.GetLogger(typeof(BienBanNTController));

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
                IBienBanNTService service = IoC.Resolve<IBienBanNTService>();
                var list = service.GetbyFilter(request.Filter.maDViQly, request.Filter.maYCau, request.Filter.keyword, request.Filter.status, fromtime, totime, pageindex, request.Paginator.pageSize, out total);

                var data = new List<BienBanNTModel>();
                foreach (var item in list)
                {
                    data.Add(new BienBanNTModel(item));
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
        [HttpGet]
        [Route("detail/{id}")]
        public IHttpActionResult Detail(int id)
        {
            try
            {
                IRepository repository = new FileStoreRepository();
                IBienBanNTService service = IoC.Resolve<IBienBanNTService>();
                var item = service.Getbykey(id);
                byte[] pdfdata = repository.GetData(item.Data);
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
        [Route("detailBBNT/{maYC}")]
        public IHttpActionResult DetailBBNT(string maYC)
        {
            try
            {
                IRepository repository = new FileStoreRepository();
                IBienBanNTService service = IoC.Resolve<IBienBanNTService>();
                var item = service.GetbyMaYeuCau(maYC);
                byte[] pdfdata = repository.GetData(item.Data);
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
        public IHttpActionResult Get(string id)
        {
            try
            {
                IYCauNghiemThuService cvservice = IoC.Resolve<IYCauNghiemThuService>();
                IBienBanNTService service = IoC.Resolve<IBienBanNTService>();
                IOrganizationService orgsrv = IoC.Resolve<IOrganizationService>();
                IBienBanDNService ttdnservice = IoC.Resolve<IBienBanDNService>();
                var congvan = cvservice.GetbyMaYCau(id);
                var ttdn = ttdnservice.GetbyNo(congvan.SoThoaThuanDN, congvan.MaYeuCau);
                if (ttdn == null)
                {
                    throw new Exception("Số thỏa thuận đấu nối không hợp lệ.");
                }

                var item = service.GetbyMaYeuCau(congvan.MaYeuCau);

                if (item == null)
                {
                    var org = orgsrv.GetbyCode(congvan.MaDViQLy);
                    item = new BienBanNT();
                    item.Data = "/bieumau/BB_NTDDongDien.pdf";
                }
                var model = new BienBanNTModel(item);

                string loaiHoSo = LoaiHSoCode.DN_NT;
                IHoSoGiayToService hsogtosrv = IoC.Resolve<IHoSoGiayToService>();
                var hsogto = hsogtosrv.GetHoSoGiayTo(congvan.MaDViQLy, congvan.MaYeuCau, loaiHoSo);
                if (hsogto == null)
                {
                    ICmisProcessService processSrv = new CmisProcessService();
                    HSoGToResult result = processSrv.GetlistHSoGTo(congvan.MaDViQLy, congvan.MaYeuCau);
                    if (result != null && result.TYPE == "OK")
                    {
                        var denghi = result.HSO_GTO.FirstOrDefault(p => p.MA_HSGT == loaiHoSo);
                        if (denghi != null)
                        {
                            hsogto = new HoSoGiayTo();
                            hsogto.MaYeuCau = denghi.MA_YCAU;
                            hsogto.LoaiHoSo = denghi.MA_HSGT;
                            hsogto.TenHoSo = denghi.TEN_HSGT;
                            hsogto.MaDViQLy = denghi.MA_DVIQLY;
                            hsogto.Data = denghi.DUONG_DAN;
                            hsogto.TrangThai = int.Parse(denghi.TINH_TRANG);
                            hsogtosrv.CreateNew(hsogto);
                            hsogtosrv.CommitChanges();
                        }
                    }
                }
                model.DeNghiNT = hsogto;
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
        [Route("CreateBBNT/{maYeuCau}")]
        public IHttpActionResult CreateBBNT(string maYeuCau)
        {
            try
            {
                IYCauNghiemThuService cvservice = IoC.Resolve<IYCauNghiemThuService>();
                IBienBanNTService bienBanNTService = IoC.Resolve<IBienBanNTService>();
                IBienBanDNService ttdnservice = IoC.Resolve<IBienBanDNService>();

                var httpRequest = HttpContext.Current.Request;
                var congvan = cvservice.GetbyMaYCau(maYeuCau);
                var ttdn = ttdnservice.GetbyNo(congvan.SoThoaThuanDN, congvan.MaYeuCau);

                if (ttdn == null)
                {
                    throw new Exception("Số thỏa thuận đấu nối không hợp lệ.");
                }
                if (ttdn != null)
                {
                    var bbnt = bienBanNTService.GetbyMaYeuCau(congvan.MaYeuCau);
                    if (bbnt == null)
                    {
                        bbnt = new BienBanNT();
                        bbnt.DiaDiemXayDung = ttdn.DiaDiemXayDung;
                        bbnt.KHChucVu = ttdn.KHChucDanh;
                        bbnt.KHDaiDien = ttdn.KHDaiDien;
                        bbnt.KHDiaChi = ttdn.KHDiaChi;
                        bbnt.KHDienThoai = ttdn.KHDienThoai;
                        bbnt.KHMa = ttdn.MaKH;
                        bbnt.KHMaSoThue = ttdn.KHMaSoThue;
                        bbnt.KHTen = ttdn.KHTen;
                        bbnt.MaCViec = ttdn.MaCViec;
                        bbnt.MaDViQLy = ttdn.MaDViQLy;
                        bbnt.MaYeuCau = ttdn.MaYeuCau;
                        bbnt.NgayLap = DateTime.Now;
                        bbnt.NgayThoaThuan = ttdn.NgayLap;
                        bbnt.SoThoaThuan = ttdn.SoBienBan;
                        bbnt.TenCongTrinh = ttdn.TenCongTrinh;
                        bbnt.ThoaThuanID = ttdn.ID;
                    }

                    //Upload Image
                    var postedFile = httpRequest.Files["File"];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        IRepository repository = new FileStoreRepository();
                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(postedFile.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(postedFile.ContentLength);
                        }
                        string folder = $"{bbnt.MaDViQLy}/{bbnt.MaYeuCau}";
                        bbnt.Data = repository.Store(folder, fileData, bbnt.Data);
                    }
                    bienBanNTService.Save(bbnt);
                    bienBanNTService.CommitChanges();
                    return Ok(new BienBanNTModel(bbnt));
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
        [Route("approve")]
        [HttpPost]
        public IHttpActionResult Approve(BienBanNTModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IHopDongService hdservice = IoC.Resolve<IHopDongService>();
                IBienBanNTService service = IoC.Resolve<IBienBanNTService>();
                var item = service.Getbykey(model.ID);
                var hopdong = hdservice.GetbyMaYCau(item.MaYeuCau);
                if (hopdong == null || hopdong.TrangThai < (int)TrangThaiBienBan.HoanThanh)
                {
                    result.success = false;
                    result.message = "Hợp đồng chưa đủ chữ ký, vui lòng hoàn thành hợp đồng.";
                    return Ok(result);
                }
                string message = "";
                result.success = service.Approve(item, out message);
                result.message = message;                
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
        [Route("UploadBBNT/{maYeuCau}")]
        public IHttpActionResult UploadBBNT(string maYeuCau)
        {
            try
            {
                IYCauNghiemThuService cvservice = IoC.Resolve<IYCauNghiemThuService>();
                IBienBanNTService bienBanNTService = IoC.Resolve<IBienBanNTService>();
                IBienBanDNService ttdnservice = IoC.Resolve<IBienBanDNService>();

                var httpRequest = HttpContext.Current.Request;
                var congvan = cvservice.GetbyMaYCau(maYeuCau);
                var ttdn = ttdnservice.GetbyNo(congvan.SoThoaThuanDN, congvan.MaYeuCau);

                if (ttdn == null)
                {
                    throw new Exception("Số thỏa thuận đấu nối không hợp lệ.");
                }
                if (ttdn != null)
                {
                    var bbnt = bienBanNTService.GetbyMaYeuCau(congvan.MaYeuCau);
                    if (bbnt == null)
                    {
                        bbnt = new BienBanNT();
                        bbnt.DiaDiemXayDung = ttdn.DiaDiemXayDung;
                        bbnt.KHChucVu = ttdn.KHChucDanh;
                        bbnt.KHDaiDien = ttdn.KHDaiDien;
                        bbnt.KHDiaChi = ttdn.KHDiaChi;
                        bbnt.KHDienThoai = ttdn.KHDienThoai;
                        bbnt.KHMa = ttdn.MaKH;
                        bbnt.KHMaSoThue = ttdn.KHMaSoThue;
                        bbnt.KHTen = ttdn.KHTen;
                        bbnt.MaCViec = ttdn.MaCViec;
                        bbnt.MaDViQLy = ttdn.MaDViQLy;
                        bbnt.MaYeuCau = ttdn.MaYeuCau;
                        bbnt.NgayLap = DateTime.Now;
                        bbnt.NgayThoaThuan = ttdn.NgayLap;
                        bbnt.SoThoaThuan = ttdn.SoBienBan;
                        bbnt.TenCongTrinh = ttdn.TenCongTrinh;
                        bbnt.ThoaThuanID = ttdn.ID;
                    }

                    //Upload Image
                    List<string> filePath = new List<string>();
                    var listPostedFile = httpRequest.Files;
                    string folder = $"{bbnt.MaDViQLy}/{bbnt.MaYeuCau}";
                    IRepository repository = new FileStoreRepository();
                    foreach (string filename in listPostedFile.AllKeys.OrderBy(x => x))
                    {
                        HttpPostedFile postedFile = httpRequest.Files[filename];
                        if (postedFile != null && postedFile.ContentLength > 0)
                        {

                            byte[] fileData = null;
                            using (var binaryReader = new BinaryReader(postedFile.InputStream))
                            {
                                fileData = binaryReader.ReadBytes(postedFile.ContentLength);
                            }

                            string path = repository.Store(folder, fileData, "", "PNG");
                            filePath.Add(path);
                        }
                    }

                    bbnt.Data = repository.CombineToPdf(folder, filePath);


                    bienBanNTService.Save(bbnt);
                    bienBanNTService.CommitChanges();
                    return Ok(new BienBanNTModel(bbnt));
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest();
            }
        }
    }
}