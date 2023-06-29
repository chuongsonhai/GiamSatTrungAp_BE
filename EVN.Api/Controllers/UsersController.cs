using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Api.Model.Request;
using EVN.Core;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(UsersController));

        [JwtAuthentication]
        [HttpPost]
        [Route("filter")]
        public IHttpActionResult Filter(UserFilterRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                int total = 0;
                IUserdataService service = IoC.Resolve<IUserdataService>();
                IList<Userdata> list = service.GetbyFilter(request.Filter.maDViQLy, request.Filter.maBPhan, request.Filter.keyword, pageindex, request.Paginator.pageSize, out total);

                result.total = total;
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
                IUserdataService service = IoC.Resolve<IUserdataService>();
                var item = service.Getbykey(id);
                UserDataRequest model = new UserDataRequest(item);
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
        public IHttpActionResult Post(UserDataRequest model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IOrganizationService orgservice = IoC.Resolve<IOrganizationService>();
                IUserdataService service = IoC.Resolve<IUserdataService>();
                var org = orgservice.GetbyCode(model.maDViQLy);
                
                var item = new Userdata();
                item = model.ToEntity(item);
                item.username = model.username;
                if (service.GetbyName(model.username)!=null)
                {
                    result.success = false;
                    result.message = "Đã tồn tại username trên hệ thống";
                    return Ok(result);
                }
                item.email = model.email;

                item.orgId = org.orgId.ToString();
                item.maDViQLy = org.orgCode;

                item.passwordsalt = GeneratorPassword.GenerateSalt();
                item.password = GeneratorPassword.EncodePassword(model.password, item.passwordsalt);
                service.CreateNew(item);
                service.CommitChanges();
                result.success = true;
                if (model.Roles != null && model.Roles.Count > 0)
                {
                    result.success = service.SaveRolesToUser(item, model.Roles.ToArray());
                }
                result.data = item;

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
        [HttpPut]
        public IHttpActionResult Put(UserDataRequest model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IUserdataService service = IoC.Resolve<IUserdataService>();
                var item = service.Getbykey(model.userId);
                item = model.ToEntity(item);
        
                if (model.password != item.password)
                {
                    item.passwordsalt = GeneratorPassword.GenerateSalt();
                    item.password = GeneratorPassword.EncodePassword(model.password, item.passwordsalt);
                }

                service.Save(item);
                service.CommitChanges();
                result.data = item;
                result.success = true;
                if (model.Roles != null && model.Roles.Count > 0)
                {
                    result.success = service.SaveRolesToUser(item, model.Roles.ToArray());
                }
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
