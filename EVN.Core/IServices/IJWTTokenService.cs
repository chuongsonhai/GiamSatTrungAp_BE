using EVN.Core.Domain;
using FX.Data;
using System.Collections.Generic;

namespace EVN.Core.IServices
{
    public interface IJWTTokenService : IBaseService<JWTToken, int>
    {
        JWTToken GetbyToken(string token);

        JWTToken GetbyUser(string userName);
    }
}