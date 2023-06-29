using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Core.CMIS;
using System;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/thongtintbi")]
    public class ThongTinTBiController : ApiController
    {
        [HttpPost]
        //[JwtAuthentication]
        [Route("thongtincto")]
        public IHttpActionResult ThongTinCTo(ThietBiRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ICmisProcessService service = new CmisProcessService();
                var list = service.GetCongTo(request.maDViQLy, request.soTBi);
                result.data = list;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
                return Ok(result);
            }
        }

        [HttpPost]
        //[JwtAuthentication]
        [Route("thongtinti")]
        public IHttpActionResult ThongTinTI(ThietBiRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ICmisProcessService service = new CmisProcessService();
                var list = service.GetTI(request.maDViQLy, request.soTBi);
                result.data = list;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
                return Ok(result);
            }
        }

        [HttpPost]
        //[JwtAuthentication]
        [Route("thongtintu")]
        public IHttpActionResult ThongTinTU(ThietBiRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ICmisProcessService service = new CmisProcessService();
                var list = service.GetTU(request.maDViQLy, request.soTBi);
                result.data = list;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
                return Ok(result);
            }
        }
    }

    public class ThietBiRequest
    {
        public string maDViQLy { get; set; } = string.Empty;
        public string MaYeuCau { get; set; } = string.Empty;
        public string soTBi { get; set; } = string.Empty;
    }
}
