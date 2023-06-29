using FX.Core;
using EVN.Api.Model;
using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using EVN.Api.Jwt;
using EVN.Api.Model.Request;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Reflection;
using log4net;
using EVN.Core;
using EVN.Core.Utilities;
using EVN.Core.Repository;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/kysootp")]
    public class KySoOTPController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(KySoOTPController));
        [JwtAuthentication]
        [HttpPost]
        [Route("laymaotp")]
        public IHttpActionResult LayMaOTP(KySoOTPRequest model, CancellationToken cancellationToken)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                CreateAndSendOtpCmisCommand request = new CreateAndSendOtpCmisCommand(model.maDonViQuanLy, "", model.soDienThoai);
                ApiResult otpresult = KySoOTPUtils.LayMaOTP(request, cancellationToken);

                result.data = otpresult.Data;
                result.success = !otpresult.IsError;
                result.message = otpresult.Message;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = "Không tìm thấy thông tin";
                return Ok(result);
            }
        }


        [JwtAuthentication]
        [HttpPost]
        [Route("xacnhanotp")]
        public IHttpActionResult XacNhanOTP(XacNhanKySoOTPRequest model, CancellationToken cancellationToken)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                VerifyOtpCmisCommand request = new VerifyOtpCmisCommand(model.sodt, model.otptoken, model.maxacnhan, model.madonvi);

                ApiResult otpresult = KySoOTPUtils.XacNhanOTP(request, cancellationToken);
                if (otpresult.IsError)
                {
                    result.success = false;
                    result.message = otpresult.Message;
                }
                else
                {
                    KySoOTPTrungApCommand kySoOTPTrungApCommand = new KySoOTPTrungApCommand();
                    kySoOTPTrungApCommand.OTP = model.maxacnhan;
                    kySoOTPTrungApCommand.MaYeuCau = model.mayeucau;
                    kySoOTPTrungApCommand.TuKhoa = model.tukhoa;
                    kySoOTPTrungApCommand.NguoiKy = model.nguoiky;
                    kySoOTPTrungApCommand.Base64File = model.base64String;
                    kySoOTPTrungApCommand.SoDienThoai = model.sodt;

                    ApiResult kysoresult = KySoOTPUtils.KySoOTP(kySoOTPTrungApCommand, cancellationToken);
                    result.data = kysoresult.Data;
                    result.success = !kysoresult.IsError;
                    result.message = kysoresult.Message;
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = "Không xác nhận được OTP";
                return Ok(result);
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("xacnhanbienbanks")]
        public IHttpActionResult XacNhanBienBanKS(XacNhanBienBanKSRequest model, CancellationToken cancellationToken)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IRepository repository = new FileStoreRepository();
                IBienBanKSService service = IoC.Resolve<IBienBanKSService>();
                VerifyOtpCmisCommand request = new VerifyOtpCmisCommand(model.sodt, model.otptoken, model.maxacnhan, model.madonvi);

                ApiResult otpresult = KySoOTPUtils.XacNhanOTP(request, cancellationToken);
                if (otpresult.IsError)
                {
                    result.success = false;
                    result.message = otpresult.Message;
                }
                else
                {
                    var bienban = service.GetbyYeuCau(model.mayeucau);
                    string maLoaiHSo = LoaiHSoCode.BB_KS;
                    var template = TemplateManagement.GetTemplate(maLoaiHSo);

                    string tukhoa = template != null && !string.IsNullOrWhiteSpace(template.ChucVuKy) ? template.ChucVuKy : "BAN KỸ THUẬT/ PHÒNG KỸ THUẬT";

                    var pdfdata = repository.GetData(bienban.Data);

                    KySoOTPTrungApCommand kySoOTPTrungApCommand = new KySoOTPTrungApCommand();
                    kySoOTPTrungApCommand.OTP = model.maxacnhan;
                    kySoOTPTrungApCommand.MaYeuCau = model.mayeucau;
                    kySoOTPTrungApCommand.TuKhoa = tukhoa;
                    kySoOTPTrungApCommand.NguoiKy = model.nguoiky;
                    kySoOTPTrungApCommand.Base64File = Convert.ToBase64String(pdfdata);
                    kySoOTPTrungApCommand.SoDienThoai = model.sodt;

                    ApiResult kysoresult = KySoOTPUtils.KySoOTP(kySoOTPTrungApCommand, cancellationToken);
                    if (!kysoresult.IsError)
                    {
                        result.data = kysoresult.Data;
                        var bienbandata = Convert.FromBase64String(kysoresult.Data);
                        result.success = service.SignRemote(bienban, bienbandata, model.macviec, model.mabphannhan, model.manviennhan, DateTime.Today, "Hoàn thành biên bản khảo sát");
                        return Ok(result);
                    }
                    result.success = false;
                    result.message = kysoresult.Message;
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = "Không xác nhận được OTP";
                return Ok(result);
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("xacnhanbienbankt")]
        public IHttpActionResult XacNhanBienBanKT(XacNhanBienBanKSRequest model, CancellationToken cancellationToken)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IRepository repository = new FileStoreRepository();
                IBienBanKTService service = IoC.Resolve<IBienBanKTService>();
                VerifyOtpCmisCommand request = new VerifyOtpCmisCommand(model.sodt, model.otptoken, model.maxacnhan, model.madonvi);

                ApiResult otpresult = KySoOTPUtils.XacNhanOTP(request, cancellationToken);
                if (otpresult.IsError)
                {
                    result.success = false;
                    result.message = otpresult.Message;
                }
                else
                {
                    var bienban = service.GetbyMaYCau(model.mayeucau);
                    string maLoaiHSo = LoaiHSoCode.BB_KT;
                    var template = TemplateManagement.GetTemplate(maLoaiHSo);

                    string tukhoa = template != null && !string.IsNullOrWhiteSpace(template.ChucVuKy) ? template.ChucVuKy : "NGƯỜI KIỂM TRA";

                    var pdfdata = repository.GetData(bienban.Data);

                    KySoOTPTrungApCommand kySoOTPTrungApCommand = new KySoOTPTrungApCommand();
                    kySoOTPTrungApCommand.OTP = model.maxacnhan;
                    kySoOTPTrungApCommand.MaYeuCau = model.mayeucau;
                    kySoOTPTrungApCommand.TuKhoa = tukhoa;
                    kySoOTPTrungApCommand.NguoiKy = model.nguoiky;
                    kySoOTPTrungApCommand.Base64File = Convert.ToBase64String(pdfdata);
                    kySoOTPTrungApCommand.SoDienThoai = model.sodt;

                    ApiResult kysoresult = KySoOTPUtils.KySoOTP(kySoOTPTrungApCommand, cancellationToken);
                    if (!kysoresult.IsError)
                    {
                        result.data = kysoresult.Data;
                        var bienbandata = Convert.FromBase64String(kysoresult.Data);
                        result.success = service.SignRemote(bienban, bienbandata);
                        return Ok(result);
                    }
                    result.success = false;
                    result.message = kysoresult.Message;
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = "Không xác nhận được OTP";
                return Ok(result);
            }
        }

    }
}
