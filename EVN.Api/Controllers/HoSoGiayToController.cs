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
using System.Linq;
using System.Web;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/hosogiayto")]
    public class HoSoGiayToController : ApiController
    {
        private readonly ILog log = LogManager.GetLogger(typeof(HoSoGiayToController));

        [JwtAuthentication]
        [HttpPost]
        [Route("listsign")]
        public IHttpActionResult ListSign(BaseRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                int total = 0;
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                IHoSoGiayToService service = IoC.Resolve<IHoSoGiayToService>();
                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);
                IList<HoSoGiayTo> model = service.ListSign(userdata.maDViQLy, request.SearchTerm, pageindex, request.Paginator.pageSize, out total);
                result.data = model;
                result.success = true;
                result.total = total;
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
        [Route("filter")]
        public IHttpActionResult Filter(TTYeuCauRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IHoSoGiayToService service = IoC.Resolve<IHoSoGiayToService>();
                IList<HoSoGiayTo> model = service.ListHSoGTo(request.maDViQLy, request.maYCau);
                result.data = model;
                result.success = true;
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
        [Route("getlist")]
        public IHttpActionResult GetList(TTYeuCauRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IHoSoGiayToService service = IoC.Resolve<IHoSoGiayToService>();
                IList<HoSoGiayTo> model = service.GetbyYeuCau(request.maDViQLy, request.maYCau);
                result.data = model;
                result.success = true;
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
                IHoSoGiayToService service = IoC.Resolve<IHoSoGiayToService>();
                var model = service.Getbykey(id);

                if (repository.GetData(model.Data) == null)
                {
                    ICmisProcessService cmisProcess = new CmisProcessService();
                    byte[] buffdata = cmisProcess.GetData(model.MaDViQLy, model.MaYeuCau, model.LoaiHoSo);
                    if (buffdata != null && buffdata.Length > 0)
                    {
                        string path = $"{model.MaDViQLy}/{model.MaYeuCau}";
                        model.Data = repository.Store(path, buffdata, loaiFile: model.LoaiFile);
                        service.Save(model);
                        service.CommitChanges();
                        return Ok(buffdata);
                    }
                    return NotFound();
                }
                return Ok(repository.GetData(model.Data));
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("GetByLoai")]
        public IHttpActionResult GetByLoai(HSoGiayToFilter request)
        {
            try
            {
                IRepository repository = new FileStoreRepository();
                IHoSoGiayToService service = IoC.Resolve<IHoSoGiayToService>();
                var model = service.GetHoSoGiayTo(request.maDViQly, request.maYCau, request.loaiHS);
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
        [Route("signdocs")]
        public IHttpActionResult SignDocs(SignDocRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IOrganizationService orgSrv = IoC.Resolve<IOrganizationService>();
                IRepository repository = new FileStoreRepository();
                IHoSoGiayToService service = IoC.Resolve<IHoSoGiayToService>();
                ICmisProcessService cmisdeliver = new CmisProcessService();
                IThoaThuanDamBaoService thoaThuanDamBaoService = IoC.Resolve<IThoaThuanDamBaoService>();
                IThoaThuanTyLeService thoaThuanTyLeService= IoC.Resolve<IThoaThuanTyLeService>();
                IChamDutHopDongService chamDutHopDongService=IoC.Resolve<IChamDutHopDongService>();


                var list = service.Query.Where(p => request.ids.Contains(p.ID) && p.TrangThai == 1 && p.MaDViQLy == request.maDViQLy && p.MaYeuCau == request.maYCau).ToList();
                if (list.Count() == 0)
                {
                    result.message = "Không tìm thấy file cần ký.";
                    result.success = false;
                    return Ok(result);
                }
                var org = orgSrv.GetbyCode(request.maDViQLy);
                string orgCode = org.compCode;
                string signName = org.daiDien;
                string folder = $"{request.maDViQLy}/{request.maYCau}/PL_HDong";
                foreach (var item in list)
                {
                    var template = TemplateManagement.GetTemplate(item.LoaiHoSo);
                    string signPosition = template != null && !string.IsNullOrWhiteSpace(template.ChucVuKy) ? template.ChucVuKy : "BÊN BÁN ĐIỆN";
                    var pdfdata = repository.GetData(item.Data);
                    if (pdfdata == null)
                    {
                        result.message = "Không tìm thấy file cần ký:" + item.TenHoSo;
                        result.success = false;
                        return Ok(result);
                    }
                    if (!string.IsNullOrWhiteSpace(item.NguoiKy)) signName = item.NguoiKy;
                    if (item.LoaiHoSo == LoaiHSoCode.PL_HD_DB)
                    {
                        var ttdb = thoaThuanDamBaoService.GetbyMaYCau(request.maYCau);
                        if (ttdb != null)
                        {
                            signName = ttdb.DaiDien;
                        }
                    }
                    if (item.LoaiHoSo == LoaiHSoCode.PL_HD_MB)
                    {
                        var tttl = thoaThuanTyLeService.GetbyMaYCau(request.maYCau);
                        if (tttl != null)
                        {
                            signName = tttl.DaiDien;
                        }
                    }
                    if (item.LoaiHoSo == LoaiHSoCode.PL_HD_CD)
                    {
                        var cdhd = chamDutHopDongService.GetbyMaYCau(request.maYCau);
                        if (cdhd != null)
                        {
                            signName = cdhd.DaiDien;
                        }
                    }
                    log.ErrorFormat("NguoiKy:", signName);
                    if (string.IsNullOrWhiteSpace(signName))
                    {
                        result.message = "Chưa có thông tin người ký:" + item.TenHoSo;
                        result.success = false;
                        return Ok(result);
                    }
                    var signResult = PdfSignUtil.SignPdf(signName, orgCode, pdfdata, signPosition);
                    if (signResult == null)
                    {
                        result.message = "Lỗi ký phụ lục:" + item.TenHoSo;
                        result.success = false;
                        return Ok(result);
                    }
                    if (!signResult.suc)
                    {
                        log.Error(signResult.msg);
                        result.message = $"Lỗi ký phụ lục:{item.TenHoSo} - {signResult.msg}";
                        result.success = false;
                        return Ok(result);
                    }
                    var data = Convert.FromBase64String(signResult.data);

                    if (!cmisdeliver.UploadPdf(item.MaDViQLy, item.MaYeuCau, data, LoaiHSoCode.PL_HD))
                    {
                        log.Error($"Lỗi upload file lên CMIS: {item.MaYeuCau} - {LoaiHSoCode.PL_HD}");
                        //return false;
                    }
                    item.TrangThai = 2;
                    item.Data = repository.Store(folder, data, item.Data);
                    service.Save(item);
                }
                service.CommitChanges();
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                return Ok(result);
            }
        }
      
        [HttpPost]
        [Route("CreateHash")]
        public IHttpActionResult CreateHash(CreateHashHopDongRequest request)
        {
            CreateHashHopDongData result = new CreateHashHopDongData();
            string msg = "";
            try
            {
                result = ApiHelper.CreateHashHopDongUSB(request.TuKhoa, request.cert, request.Base64File, out msg);
                result.msg = msg;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.suc = false;
                result.msg = ex.Message;
                return Ok(result);
            }
        }

    
        [HttpPost]
        [Route("WrapFile")]
        public IHttpActionResult WrapFile(HopDongInsertHashedFileRequest request)
        {
            HopDongInsertHashedFileData result = new HopDongInsertHashedFileData();
            string msg = "";
            try
            {
                result = ApiHelper.HopDongInsertHashedFile_USB(request.id_file, request.hashed, out msg);
                result.msg = msg;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.suc = false;
                result.msg = ex.Message;
                return Ok(result);
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("download/{id}")]
        public IHttpActionResult Export(int id)
        {
            try
            {
                IRepository repository = new FileStoreRepository();
                IHoSoGiayToService service = IoC.Resolve<IHoSoGiayToService>();
                var model = service.Getbykey(id);
                if (string.IsNullOrWhiteSpace(model.Data))
                    return NotFound();
                return Ok(repository.GetData(model.Data));
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }
    }
}
