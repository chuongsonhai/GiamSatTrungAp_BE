using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Api.Model.Request;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/EmailZalo")]
    public class EmailZaloController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(DonViController));
        [HttpPost]
        [Route("email/add")]
        public IHttpActionResult AddEmail([FromBody] EmailModelFilter request)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IEmailService service = IoC.Resolve<IEmailService>();
                Email email = new Email();
                email.ID = request.Filter.ID;
                email.MA_DVIQLY = request.Filter.MA_DVIQLY;
                email.MA_DVU = request.Filter.MA_DVU;
                email.MA_KHANG = request.Filter.MA_KHANG;
                email.NOI_DUNG = request.Filter.NOI_DUNG;
                email.NGAY_SUA = request.Filter.NGAY_SUA;
                email.NGUOI_SUA = request.Filter.NGUOI_SUA;
                email.NGAY_TAO = request.Filter.NGAY_TAO;
                email.NGUOI_TAO = request.Filter.NGUOI_TAO;
                email.TIEU_DE = request.Filter.TIEU_DE;
                email.TINH_TRANG = request.Filter.TINH_TRANG;
                email.ID_HDON = request.Filter.ID_HDON;
                email.EMAIL = request.Filter.EMAIL;
                service.CreateNew(email);
                service.CommitChanges();
                result.success = true;
                result.message = "Thêm mới thành công";
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

        [HttpGet]
        [Route("sendnotification")]
        public IHttpActionResult Send()
        {
            ResponseFileResult result = new ResponseFileResult();
            ICanhBaoService CBservice = IoC.Resolve<ICanhBaoService>();
            IEmailService service = IoC.Resolve<IEmailService>();
            IZaloService zaloservice = IoC.Resolve<IZaloService>();
            IUserNhanCanhBaoService userNhanCanhBaoService = IoC.Resolve<IUserNhanCanhBaoService>();
            IUserdataService userdataService = IoC.Resolve<IUserdataService>();
            try
            {
                
                var listCB = CBservice.Query.Where(p => p.TRANGTHAI_CANHBAO == 1).ToList();
                foreach (var item in listCB)
                {
                    IList<UserNhanCanhBao> listNguoiNhan = userNhanCanhBaoService.GetbyMaDviQly(item.DONVI_DIENLUC);
                    foreach (var nguoiNhan in listNguoiNhan)
                    {
                        var user = userdataService.Getbykey(nguoiNhan.USER_ID);
                        Email email = new Email();
                        email.MA_DVIQLY = item.DONVI_DIENLUC;
                        email.MA_DVU = "TA";
                        email.NOI_DUNG = item.NOIDUNG;
                        email.NGAY_TAO = DateTime.Now;
                        email.NGUOI_TAO = "admin";
                        email.TIEU_DE = "Cảnh báo giám sát cấp điện trung áp";
                        email.TINH_TRANG = 1;
                        email.EMAIL = user.email;
                        service.CreateNew(email);
                     
                    }
                    foreach (var nguoiNhan in listNguoiNhan)
                    {
                        Zalo zalo = new Zalo(); ;
                        zalo.MA_DVIQLY = item.DONVI_DIENLUC;
                        zalo.MA_DVU = "TA";
                        zalo.NOI_DUNG = item.NOIDUNG;
                        zalo.NGAY_TAO = DateTime.Now;
                        zalo.NGUOI_TAO = "admin";
                        zalo.TIEU_DE = "Cảnh báo giám sát cấp điện trung áp";
                        zalo.TINH_TRANG = 1;
                        zalo.ID_ZALO = "1234";
                        zaloservice.CreateNew(zalo);
                    }
                        item.TRANGTHAI_CANHBAO = 2;
                    CBservice.Update(item);
                }

                
                service.CommitChanges();
                result.success = true;
                result.message = "Thêm mới thành công";
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

        //[HttpPost]
        //[Route("zalo/add")]
        //public IHttpActionResult AddZalo([FromBody] ZaloModelFilter request)
        //{
        //    ResponseFileResult result = new ResponseFileResult();
        //    try
        //    {
        //        IZaloService service = IoC.Resolve<IZaloService>();
        //        Zalo zalo = new Zalo();
        //        zalo.ID = request.Filter.ID;
        //        zalo.MA_DVIQLY = request.Filter.MA_DVIQLY;
        //        zalo.MA_DVU = request.Filter.MA_DVU;
        //        zalo.MA_KHANG = request.Filter.MA_KHANG;
        //        zalo.NOI_DUNG = request.Filter.NOI_DUNG;
        //        zalo.NGAY_SUA = request.Filter.NGAY_SUA;
        //        zalo.NGUOI_SUA = request.Filter.NGUOI_SUA;
        //        zalo.NGAY_TAO = request.Filter.NGAY_TAO;
        //        zalo.NGUOI_TAO = request.Filter.NGUOI_TAO;
        //        zalo.TIEU_DE = request.Filter.TIEU_DE;
        //        zalo.TINH_TRANG = request.Filter.TINH_TRANG;
        //        zalo.ID_ZALO = request.Filter.ID_ZALO;
        //        zalo.ID_HDON = request.Filter.ID_HDON;
        //        service.CreateNew(zalo);
        //        service.CommitChanges();
        //        result.success = true;
        //        result.message = "Thêm mới thành công";
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex);
        //        result.success = false;
        //        result.message = ex.Message;
        //        return Ok(result);
        //    }
        //}
    }
}
