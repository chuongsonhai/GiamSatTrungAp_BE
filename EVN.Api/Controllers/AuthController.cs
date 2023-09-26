using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Core.CMIS;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(AuthController));

        [AllowAnonymous]
        [HttpPost]
        [Route("authen")]
        public IHttpActionResult Authen(AuthModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                log.Error(model.ticket);
                IUserdataService service = IoC.Resolve<IUserdataService>();
                IOrganizationService orgservice = IoC.Resolve<IOrganizationService>();

                var userdata = service.GetbyTicket(model.ticket);
                if (userdata == null)
                    throw new Exception("Sai tên đăng nhập hoặc mật khẩu");

                userdata = service.Getbykey(userdata.userId);
                var org = orgservice.Getbykey(int.Parse(userdata.orgId));
                var roles = userdata.Roles.Select(p => p.groupName).ToArray();
                var token = JwtManager.GenerateToken(userdata.username, roles);
                var data = new UserModel() { userId = userdata.userId, username = userdata.username, fullName = userdata.fullName, email = userdata.email, maDViQLy = org.orgCode, JwtToken = token.Token, RefreshToken = token.RefreshToken };
                data.Roles = new List<RoleModel>();
                foreach (var role in userdata.Roles)
                    data.Roles.Add(new RoleModel(role));
                result.data = data;
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

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login(LoginModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IUserdataService service = IoC.Resolve<IUserdataService>();
                IOrganizationService orgservice = IoC.Resolve<IOrganizationService>();

                var userdata = service.Authenticate(model.Username, model.Password);
                if (userdata == null) throw new Exception("Sai tên đăng nhập hoặc mật khẩu");

                if (!string.IsNullOrWhiteSpace(model.notifyid))
                {
                    userdata.NotifyId = model.notifyid;
                    service.Save(userdata);
                    service.CommitChanges();
                }

                var org = orgservice.Getbykey(int.Parse(userdata.orgId));

                var token = JwtManager.GenerateToken(userdata.username, userdata.Roles.Select(p => p.groupName).ToArray());
                var data = new UserModel() { userId = userdata.userId, username = userdata.username, fullName = userdata.fullName, email = userdata.email, maDViQLy = org.orgCode, JwtToken = token.Token, RefreshToken = token.RefreshToken };
                data.Roles = new List<RoleModel>();
                foreach (var item in userdata.Roles)
                    data.Roles.Add(new RoleModel(item));
                result.data = data;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = ex.Message;
                return Ok(result);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);
                var data = new UserModel() { userId = userdata.userId, username = userdata.username, fullName = userdata.fullName, email = userdata.email };
                data.Roles = new List<RoleModel>();
                foreach (var item in userdata.Roles)
                    data.Roles.Add(new RoleModel(item));
                return Ok(data);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest();
            }
        }
    }
}
