using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Core;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using System;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/phancongtc")]
    public class PhanCongTCController : ApiController
    {
        private readonly ILog log = LogManager.GetLogger(typeof(PhanCongTCController));
        [JwtAuthentication]
        [HttpPost]
        [Route("getdata")]
        public IHttpActionResult GetData(PhanCongTCRequest request)
        {
            try
            {
                IPhanCongTCService service = IoC.Resolve<IPhanCongTCService>();
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();

                var congvan = congvansrv.GetbyMaYCau(request.maYeuCau);
                var item = service.GetbyMaYCau(congvan.MaLoaiYeuCau, congvan.MaYeuCau, request.loai);

                if (item == null)
                {
                    item = new PhanCongTC();
                    item.MA_DVIQLY = congvan.MaDViQLy;
                    
                    item.MA_YCAU_KNAI = congvan.MaYeuCau;
                    item.MA_DDO_DDIEN = congvan.MaDDoDDien;

                    item.NGAY_BDAU = DateTime.Today;
                    item.NGAY_HEN = DateTime.Now;

                    item.MA_LOAI_YCAU = congvan.MaLoaiYeuCau;
                    item.MA_CVIEC_TRUOC = "PC";
                    item.LOAI = request.loai;
                }
                PhanCongTCModel model = new PhanCongTCModel(item);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        public IHttpActionResult Post([FromBody] PhanCongTCModel model)
        {
            IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
            IPhanCongTCService service = IoC.Resolve<IPhanCongTCService>();
            IBienBanDNService ttdnservice = IoC.Resolve<IBienBanDNService>();

            var congvan = congvansrv.GetbyMaYCau(model.MA_YCAU_KNAI);
            var ttdn = ttdnservice.GetbyNo(congvan.SoThoaThuanDN, congvan.MaYeuCau);

            model.LOAI = 1;
            if (string.IsNullOrWhiteSpace(model.NDUNG_XLY))
                model.NDUNG_XLY = "Phân công thi công, treo tháo";

            PhanCongTC item = new PhanCongTC();
            item = model.ToEntity(item);
            if (item.TRANG_THAI == 0)
            {
                service.CreateNew(item);
                service.CommitChanges();
                return Ok(item);
            }
            if (!service.SavePhanCong(ttdn, item))
                return BadRequest();
            return Ok(new PhanCongTCModel(item));
        }

        [JwtAuthentication]
        [HttpPut]
        public IHttpActionResult Put([FromBody] PhanCongTCModel model)
        {
            IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
            IPhanCongTCService service = IoC.Resolve<IPhanCongTCService>();
            IBienBanDNService ttdnservice = IoC.Resolve<IBienBanDNService>();

            var congvan = congvansrv.GetbyMaYCau(model.MA_YCAU_KNAI);
            var ttdn = ttdnservice.GetbyNo(congvan.SoThoaThuanDN, congvan.MaYeuCau);

            model.LOAI = 1;
            if (string.IsNullOrWhiteSpace(model.NDUNG_XLY))
                model.NDUNG_XLY = "Phân công thi công, treo tháo";

            var item = service.Getbykey(model.ID);
            item = model.ToEntity(item);
            if (item.TRANG_THAI == 0)
            {
                service.Save(item);
                service.CommitChanges();
                return Ok(item);
            }
            if (!service.SavePhanCong(ttdn, item))
                return BadRequest();
            return Ok(new PhanCongTCModel(item));
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("cancel")]
        public IHttpActionResult Cancel([FromBody] PhanCongTCModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                IPhanCongTCService service = IoC.Resolve<IPhanCongTCService>();

                var congvan = congvansrv.GetbyMaYCau(model.MA_YCAU_KNAI);
                PhanCongTC item = service.Getbykey(model.ID);
                if (congvan.TrangThai > TrangThaiNghiemThu.BienBanTC)
                {
                    log.Error(congvan.TrangThai);
                    result.message = "Không được phân công lại, yêu cầu đã ghi nhận kết quả treo tháo";
                    result.success = false;
                    return Ok(result);
                }
                if (!service.CancelThiCong(congvan, item))
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
