using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class JWTToken
    {
        public virtual int ID { get; set; }
        public virtual string Token { get; set; }
        public virtual string RefreshToken { get; set; }
        public virtual DateTime ExpiredDate { get; set; }
        public virtual string UserName { get; set; }
    }
}