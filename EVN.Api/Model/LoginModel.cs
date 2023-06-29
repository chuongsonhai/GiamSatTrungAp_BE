using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model
{
    public class AuthModel
    {
        public string ticket { get; set; }
        public string notifyid { get; set; }
        public string deviceid { get; set; }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string notifyid { get; set; }
        public string deviceid { get; set; }
    }

    public class ChangePasswordModel
    {
        public string password { get; set; }
        public string newpassword { get; set; }
        public string confirmpassword { get; set; }
    }
}
