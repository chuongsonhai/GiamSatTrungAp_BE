using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class JWTTokenService : BaseService<JWTToken, int>, IJWTTokenService
    {
        public JWTTokenService(string sessionFactoryConfigPath)
           : base(sessionFactoryConfigPath)
        { }

        public JWTToken GetbyToken(string token)
        {
            return Query.Where(p => p.Token == token).FirstOrDefault();
        }

        public JWTToken GetbyUser(string userName)
        {
            return Query.Where(p => p.UserName == userName).FirstOrDefault();
        }
    }
}