using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Core;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/phancongks")]
    public class PhanCongKSController : ApiController
    {
        private readonly ILog log = LogManager.GetLogger(typeof(PhanCongKSController));

        [JwtAuthentication]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            try
            {
                IPhanCongKSService service = IoC.Resolve<IPhanCongKSService>();
                ICongVanYeuCauService congvansrv = IoC.Resolve<ICongVanYeuCauService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                ITThaiYeuCauService ttycausrv = IoC.Resolve<ITThaiYeuCauService>();
                var tthaiycau = ttycausrv.GetbyStatus((int)TrangThaiCongVan.PhanCongKS, 0);

                var congvan = congvansrv.Getbykey(id);
                var item = service.GetbyMaYCau(congvan.MaLoaiYeuCau, congvan.MaYeuCau);
                if (item == null)
                {
                    item = new PhanCongKS();
                    item.MA_DVIQLY = congvan.MaDViQLy;

                    item.MA_LOAI_YCAU = congvan.MaLoaiYeuCau;
                    item.MA_YCAU_KNAI = congvan.MaYeuCau;
                    item.MA_DDO_DDIEN = congvan.MaDDoDDien;

                    item.NGAY_HEN = DateTime.Now;
                    item.NGAY_BDAU = DateTime.Today;

                    item.MA_CVIEC_TRUOC = tthaiycau.CVIEC_TRUOC;
                }
                PhanCongKSModel model = new PhanCongKSModel(item);                
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
        [Route("GetByMaYC/{maYC}")]
        public IHttpActionResult GetByMaYC(string  maYC)
        {
            try
            {
                IPhanCongKSService service = IoC.Resolve<IPhanCongKSService>();
                ICongVanYeuCauService congvansrv = IoC.Resolve<ICongVanYeuCauService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                ITThaiYeuCauService ttycausrv = IoC.Resolve<ITThaiYeuCauService>();
                var tthaiycau = ttycausrv.GetbyStatus((int)TrangThaiCongVan.PhanCongKS, 0);

                var congvan = congvansrv.GetbyMaYCau(maYC);
                var item = service.GetbyMaYCau(congvan.MaLoaiYeuCau, congvan.MaYeuCau);
                if (item == null)
                {
                    item = new PhanCongKS();
                    item.MA_DVIQLY = congvan.MaDViQLy;

                    item.MA_LOAI_YCAU = congvan.MaLoaiYeuCau;
                    item.MA_YCAU_KNAI = congvan.MaYeuCau;
                    item.MA_DDO_DDIEN = congvan.MaDDoDDien;

                    item.NGAY_HEN = DateTime.Now;
                    item.NGAY_BDAU = DateTime.Today;

                    item.MA_CVIEC_TRUOC = tthaiycau.CVIEC_TRUOC;
                }
                PhanCongKSModel model = new PhanCongKSModel(item);
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
        public IHttpActionResult Post([FromBody] PhanCongKSModel model)
        {
            ICongVanYeuCauService congvansrv = IoC.Resolve<ICongVanYeuCauService>();
            IPhanCongKSService service = IoC.Resolve<IPhanCongKSService>();

            if (string.IsNullOrWhiteSpace(model.NDUNG_XLY))
                model.NDUNG_XLY = "Phân công khảo sát lưới điện";

            var congvan = congvansrv.GetbyMaYCau(model.MA_YCAU_KNAI);
            PhanCongKS item = new PhanCongKS();
            item = model.ToEntity(item);

            PhanCongKSModel result = new PhanCongKSModel();
            if (item.TRANG_THAI == 0)
            {
                service.CreateNew(item);
                service.CommitChanges();
                result = new PhanCongKSModel(item);
                return Ok(result);
            }
            if (!service.SavePhanCong(congvan, item))
                return BadRequest();
            result = new PhanCongKSModel(item);            
            return Ok(result);
        }

        [JwtAuthentication]
        [HttpPut]
        public IHttpActionResult Put([FromBody] PhanCongKSModel model)
        {
            ICongVanYeuCauService congvansrv = IoC.Resolve<ICongVanYeuCauService>();
            IPhanCongKSService service = IoC.Resolve<IPhanCongKSService>();

            if (string.IsNullOrWhiteSpace(model.NDUNG_XLY))
                model.NDUNG_XLY = "Phân công khảo sát lưới điện";

            var congvan = congvansrv.GetbyMaYCau(model.MA_YCAU_KNAI);
            PhanCongKS item = service.Getbykey(model.ID);
            item = model.ToEntity(item);
            PhanCongKSModel result = new PhanCongKSModel();
            if (item.TRANG_THAI == 0)
            {
                service.Save(item);
                service.CommitChanges();
                result = new PhanCongKSModel(item);
                return Ok(result);
            }
            if (!service.SavePhanCong(congvan, item))
                return BadRequest();
            result = new PhanCongKSModel(item);
            return Ok(result);
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("cancel")]
        public IHttpActionResult Cancel([FromBody] PhanCongKSModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ICongVanYeuCauService congvansrv = IoC.Resolve<ICongVanYeuCauService>();
                IPhanCongKSService service = IoC.Resolve<IPhanCongKSService>();

                var congvan = congvansrv.GetbyMaYCau(model.MA_YCAU_KNAI);
                PhanCongKS item = service.Getbykey(model.ID);                
                if (congvan.TrangThai > TrangThaiCongVan.GhiNhanKS)
                {
                    log.Error(congvan.TrangThai);
                    result.message = "Không được phân công lại, yêu cầu đã ghi nhận kết quả khảo sát";
                    result.success = false;
                    return Ok(result);
                }
                if (!service.Cancel(congvan, item))
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
