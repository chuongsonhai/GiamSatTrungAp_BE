using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using System;
using System.Linq;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/donvi")]
    public class DonViController : ApiController
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
                IOrganizationService service = IoC.Resolve<IOrganizationService>();
                
                var list = service.Query.ToList();
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
                IOrganizationService service = IoC.Resolve<IOrganizationService>();
                var item = service.Getbykey(id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }

        public IHttpActionResult Put([FromBody]Organization model)
        {
            try
            {
                IOrganizationService service = IoC.Resolve<IOrganizationService>();
                service.Save(model);
                service.CommitChanges();
                return Ok(model);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest("Có lỗi xảy ra, vui lòng thực hiện lại");
            }
        }
    }
}
