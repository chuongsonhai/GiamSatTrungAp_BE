using EVN.Api.Jwt;
using EVN.Api.Model;
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
    [RoutePrefix("api/cauhinhdk")]
    public class CauHinhDKController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(DonViController));

        [JwtAuthentication]
        [HttpPost]
        [Route("filter")]
        public IHttpActionResult Filter()
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ICauHinhDKService service = IoC.Resolve<ICauHinhDKService>();
                var list = service.Query.OrderBy(p => p.ORDERNUMBER).ToList();
                result.data = list;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại";
                return Ok(result);
            }
        }

        [JwtAuthentication]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            try
            {
                ICauHinhDKService service = IoC.Resolve<ICauHinhDKService>();
                var model = service.Getbykey(id);
                return Ok(model);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpPut]
        public IHttpActionResult Put([FromBody] CauHinhDK model)
        {
            try
            {
                ICauHinhDKService service = IoC.Resolve<ICauHinhDKService>();
                model.NGAY_HLUC = DateTime.Now;
                service.Save(model);
                service.CommitChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return Ok(model);
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("GetByMaYC")]
        public IHttpActionResult GetByMaCV(TTrinhRequest request)
        {
            ICongVanYeuCauService ycaudnservice = IoC.Resolve<ICongVanYeuCauService>();
            IYCauNghiemThuService ycauntservice = IoC.Resolve<IYCauNghiemThuService>();
            ICauHinhDKService service = IoC.Resolve<ICauHinhDKService>();
            ICongViecService congviecsrv = IoC.Resolve<ICongViecService>();
            ITThaiYeuCauService ttycausrv = IoC.Resolve<ITThaiYeuCauService>();
            ResponseResult result = new ResponseResult();
            try
            {

                var ycaudn = ycaudnservice.GetbyMaYCau(request.maYCau);
                string maCViec = ycaudn.MaCViec;
                string maLoaiYCau = ycaudn.MaLoaiYeuCau;
                string maDViQLy = ycaudn.MaDViQLy;
                var tthaiycau = ttycausrv.GetbyStatus((int)ycaudn.TrangThai, 0);
                if (request.nghiemThu)
                {
                    var ycaunt = ycauntservice.GetbyMaYCau(request.maYCau);
                    maCViec = ycaunt.MaCViec;
                    tthaiycau = ttycausrv.GetbyStatus((int)ycaunt.TrangThai, 1);
                }

                var maCViecs = new List<string>();
                if (tthaiycau != null)
                    maCViecs = tthaiycau.CVIEC.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                var list = service.GetbyMaCViec(maLoaiYCau, maCViec);
                if (maCViecs.Count() > 0)
                    list = list.Where(p => maCViecs.Contains(p.MA_CVIEC_TIEP)).OrderBy(p => p.ORDERNUMBER).ToList();
       
                result.data = list;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại";
                return Ok(result);
            }
        }
    }
}
