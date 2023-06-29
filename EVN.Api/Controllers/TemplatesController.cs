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
    [RoutePrefix("api/templates")]
    public class TemplatesController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(TemplatesController));

        [JwtAuthentication]
        [HttpPost]
        [Route("filter")]
        public IHttpActionResult Filter()
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ITemplateService service = IoC.Resolve<ITemplateService>();
                var list = service.Query.ToList();
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
        public IHttpActionResult Get(string id)
        {
            try
            {
                ITemplateService service = IoC.Resolve<ITemplateService>();
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
        public IHttpActionResult Post(Template model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ITemplateService service = IoC.Resolve<ITemplateService>();
                if (service.Getbykey(model.Code) != null)
                {
                    result.success = false;
                    result.message = $"Mã biên bản {model.Code} đã có trên hệ thống";
                    return Ok(result);
                }
                service.Save(model);
                service.CommitChanges();
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
        [HttpPut]
        public IHttpActionResult Put(Template model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ITemplateService service = IoC.Resolve<ITemplateService>();
                var item = service.Getbykey(model.Code);
                if (item == null)
                {
                    item = new Template();
                    item.Code = model.Code;
                }
                item.ChucVuKy = model.ChucVuKy;
                item.XsltData = model.XsltData;
                item.XmlData = model.XmlData;
                service.Save(item);
                service.CommitChanges();
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
        [HttpPost]
        [Route("xoamau/{id}")]
        public IHttpActionResult XoaMau(string id)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ITemplateService service = IoC.Resolve<ITemplateService>();
                var item = service.Getbykey(id);
                if (item.TrangThai == 0)
                {
                    result.message = "Mẫu đang sử dụng, không được xóa";
                    result.success = false;
                    return Ok(result);
                }    
                service.Save(item);
                service.CommitChanges();
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

        [JwtAuthentication]
        [HttpPost]
        [Route("updatestatus/{id}")]
        public IHttpActionResult UpdateStatus(string id)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ITemplateService service = IoC.Resolve<ITemplateService>();
                var item = service.Getbykey(id);
                if (item.TrangThai == 0)
                    item.TrangThai = 1;
                else
                    item.TrangThai = 0;
                service.Save(item);
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
    }
}