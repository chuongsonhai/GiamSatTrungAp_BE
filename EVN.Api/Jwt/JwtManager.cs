using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace EVN.Api.Jwt
{
    public static class JwtManager
    {
        /// <summary>
        /// Use the below code to generate symmetric Secret Key
        ///     var hmac = new HMACSHA256();
        ///     var key = Convert.ToBase64String(hmac.Key);
        /// </summary>
        ///

        private static ILog log = LogManager.GetLogger(typeof(JwtManager));

        private static object looker = new object();

        /// <summary>
        /// tạo jwt token
        /// nếu chưa có tạo mới vào db
        /// nếu có rồi thì ra hạn và update vào db
        /// </summary>
        /// <param name="comId"></param>
        /// <param name="username"></param>
        /// <param name="expireMinutes"></param>
        /// <returns></returns>
        public static JWTToken GenerateToken(string username, string[] roles)
        {
            try
            {
                string token = null;
                var now = DateTime.UtcNow;
                var service = IoC.Resolve<IJWTTokenService>();
                //Kiếm tra token của user đã tồn tại trên hệ thống hay chưa
                //Nếu có rồi thì cập nhật với ID cũ
                //Nếu chưa có thì tạo mới 1 bản ghi
                var cfgservice = IoC.Resolve<ISystemConfigService>();
                var mySecretConfigs = cfgservice.GetDictionary("Secret");
                var Secret = mySecretConfigs["Secret"];

                lock (looker)
                {

                    JWTToken jwtToken = service.GetbyUser(username);
                    if (jwtToken == null) jwtToken = new JWTToken();
                    jwtToken.UserName = username.Trim().ToLower();

                    // generate jwt token
                    var symmetricKey = Convert.FromBase64String(Secret);
                    var tokenHandler = new JwtSecurityTokenHandler();

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Claims = new Dictionary<string, object>(),
                        Expires = now.AddHours(8),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
                    };

                    tokenDescriptor.Claims.Add(ClaimTypes.Name, username);
                    tokenDescriptor.Claims.Add(ClaimTypes.Role, roles);

                    SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
                    token = tokenHandler.WriteToken(securityToken);

                    // save token
                    jwtToken.Token = token;
                    jwtToken.RefreshToken = GenerateRefreshToken();
                    jwtToken.ExpiredDate = securityToken.ValidTo;

                    service.Save(jwtToken);
                    service.CommitChanges();
                    return jwtToken;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }

        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var service = IoC.Resolve<IJWTTokenService>();
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                var cfgservice = IoC.Resolve<ISystemConfigService>();
                var mySecretConfigs = cfgservice.GetDictionary("Secret");
                var Secret = mySecretConfigs["Secret"];

                if (jwtToken == null)
                {
                    log.Error($"jwtToken null: {token}");
                    JWTToken jwt = service.GetbyToken(token);
                    if (jwt == null) return null;
                    if (jwt.ExpiredDate < DateTime.Now) return null;
                }

                var symmetricKey = Convert.FromBase64String(Secret);

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

                return principal;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }
    }
}