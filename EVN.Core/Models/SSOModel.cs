using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core
{
    public class SSOResponse
    {
        public string code { get; set; }
        public string paramCode { get; set; }
        public string message { get; set; }
        public string status { get; set; }
        public SSOUser data { get; set; }        
    }
    public class SSOUser
    {
        public string serviceTicket { get; set; }
        public string expiresIn { get; set; }
        public Userdata identity { get; set; }
        public List<Role> listGroup { get; set; }
    }
}
