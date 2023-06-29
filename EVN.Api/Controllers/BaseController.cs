using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    public class BaseController : ApiController
    {
        protected Userdata CurrentUser
        {
            get
            {
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);
                return userdata;
            }
        }
    }
}
