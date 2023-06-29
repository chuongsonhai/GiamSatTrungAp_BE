using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Core.IServices;
using FX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/token")]
    public class TokenController : ApiController
    {
        [HttpPost]
        [Route("refresh")]
        public IHttpActionResult Refresh(TokenApiModel tokenApiModel)
        {
            try
            {
                if (tokenApiModel is null)
                {
                    return BadRequest("Invalid client request");
                }

                string accessToken = tokenApiModel.AccessToken;
                string refreshToken = tokenApiModel.RefreshToken;

                var principal = JwtManager.GetPrincipal(accessToken);
                var username = principal.Identity.Name; //this is mapped to the Name claim by default

                IUserdataService service = IoC.Resolve<IUserdataService>();
                var jwtservice = IoC.Resolve<IJWTTokenService>();

                var user = service.GetbyName(username);
                if (user == null)
                {
                    return BadRequest("Invalid client request");
                }

                var jwt = jwtservice.GetbyToken(accessToken);

                if (jwt == null || jwt.RefreshToken != refreshToken || jwt.ExpiredDate <= DateTime.Now)
                {
                    return BadRequest("Invalid client request");
                }

                var token = JwtManager.GenerateToken(user.username, user.Roles.Select(p => p.groupName).ToArray());
                var data = new UserModel() { userId = user.userId, username = user.username, fullName = user.fullName, email = user.email, maDViQLy = user.maDViQLy, JwtToken = token.Token, RefreshToken = token.RefreshToken };
                data.Roles = new List<RoleModel>();
                foreach (var role in user.Roles)
                    data.Roles.Add(new RoleModel(role));
                return Ok(new
                {
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid client request");
            }
        }

        [HttpPost, Authorize]
        [Route("revoke")]
        public IHttpActionResult Revoke()
        {
            var username = User.Identity.Name;

            IUserdataService service = IoC.Resolve<IUserdataService>();
            var jwtservice = IoC.Resolve<IJWTTokenService>();

            var user = service.GetbyName(username);
            if (user == null)
            {
                return BadRequest("Invalid client request");
            }

            var jwt = jwtservice.GetbyUser(username);

            if (jwt == null)
            {
                return BadRequest("Invalid client request");
            }
            jwt.RefreshToken = null;

            jwtservice.Save(jwt);
            jwtservice.CommitChanges();
            return Ok();
        }
    }

    public class TokenApiModel
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
