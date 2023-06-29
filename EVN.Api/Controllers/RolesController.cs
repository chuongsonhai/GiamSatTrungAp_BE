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
    [RoutePrefix("api/roles")]
    public class RolesController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(RolesController));

        [JwtAuthentication]
        [HttpPost]
        [Route("filter")]
        public IHttpActionResult Filter()
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IRoleService service = IoC.Resolve<IRoleService>();
                IList<Role> list = service.Query.ToList();
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
                IRoleService service = IoC.Resolve<IRoleService>();
                var item = service.Getbykey(id);
                RoleModel model = new RoleModel(item);
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
        public IHttpActionResult Post([FromBody] RoleModel model)
        {
            try
            {
                IRoleService service = IoC.Resolve<IRoleService>();
                IPermissionService perservice = IoC.Resolve<IPermissionService>();
                var role = service.GetByGroupName(model.groupName);
                if (role != null)
                {
                    log.Error($"{model.groupName} đã có trên hệ thống");
                    return BadRequest();
                }
                else
                {
                    role = new Role();
                    role.groupName = model.groupName;
                    role.description = model.description;
                    role.isSysadmin = model.isSysadmin;
                    if (model.Permissions != null && model.Permissions.Count > 0)
                        role.Permissions = perservice.Query.Where(p => model.Permissions.Contains(p.Code)).ToList();
                    service.CreateNew(role);
                    service.CommitChanges();
                    return Ok(model);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest();
            }
        }

        [JwtAuthentication]
        [HttpPut]
        public IHttpActionResult Put([FromBody] RoleModel model)
        {
            try
            {
                IRoleService service = IoC.Resolve<IRoleService>();
                IPermissionService perservice = IoC.Resolve<IPermissionService>();

                var role = service.Getbykey(model.groupId);
                role.description = model.description;
                role.isSysadmin = model.isSysadmin;
                if (model.Permissions != null && model.Permissions.Count > 0)
                    role.Permissions = perservice.Query.Where(p => model.Permissions.Contains(p.Code)).ToList();
                service.Save(role);
                service.CommitChanges();
                return Ok(model);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest();
            }
        }
    }
}
