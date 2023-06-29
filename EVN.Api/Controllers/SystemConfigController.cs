using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Context.Security;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/systemconfig")]
    public class SystemConfigController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(SystemConfigController));

        [JwtAuthentication]
        [HttpPost]
        [Route("filter")]
        public IHttpActionResult Filter()
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ISystemConfigService service = IoC.Resolve<ISystemConfigService>();
                IList<SystemConfig> list = service.Query.ToList();
                result.data = list;
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
        public IHttpActionResult Get(int id)
        {
            try
            {
                ISystemConfigService service = IoC.Resolve<ISystemConfigService>();
                var item = service.Getbykey(id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        public IHttpActionResult Post([FromBody] SystemConfig model)
        {
            try
            {
                ISystemConfigService service = IoC.Resolve<ISystemConfigService>();
                var entity = service.GetbyCode(model.Code);
                if (entity != null)
                {
                    log.Error($"{model.Code} đã có trên hệ thống");
                    return BadRequest();
                }
                entity.Code = model.Code;
                entity.Value = model.Value;
                service.CreateNew(entity);
                service.CommitChanges();
                return Ok(entity);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest();
            }
        }

        [JwtAuthentication]
        [HttpPut]
        public IHttpActionResult Put([FromBody] SystemConfig model)
        {
            try
            {
                ISystemConfigService service = IoC.Resolve<ISystemConfigService>();

                var entity = service.Getbykey(model.ID);
                entity.Value = model.Value;
                service.Save(entity);
                service.CommitChanges();
                return Ok(entity);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest();
            }
        }
    }
}
