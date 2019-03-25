using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using MilenioApi.DAO;
using System.Web.Script.Serialization;
using MilenioApi.Models;
using MilenioApi.Controllers;
using System.Configuration;

namespace WebApi.Jwt
{
    public static class JwtManager
    {
        /// <summary>
        /// Use the below code to generate symmetric Secret Key
        ///     var hmac = new HMACSHA256();
        ///     var key = Convert.ToBase64String(hmac.Key);
        /// </summary>

        public static string GenerateToken(string login, string userid, List<string> roles, Guid? entidad_id)
        {
            // appsetting for Token JWT
            var secretKey = ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
            var audienceToken = ConfigurationManager.AppSettings["JWT_AUDIENCE_TOKEN"];
            var issuerToken = ConfigurationManager.AppSettings["JWT_ISSUER_TOKEN"];
            var expireTime = ConfigurationManager.AppSettings["JWT_EXPIRE_MINUTES"];

            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            
            // create a claimsIdentity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name, login),
                new Claim(ClaimTypes.NameIdentifier, userid)
                ,new Claim(ClaimTypes.PrimaryGroupSid, entidad_id.ToString())                
            });

            //recorremos la lista de roles que se envian, y se agrega un rol por cada uno
            foreach (var r in roles)
            {
                Claim cc = new Claim(ClaimTypes.Role, r);
                claimsIdentity.AddClaim(cc);
            }

            // create token to the user
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.CreateJwtSecurityToken(
                audience: audienceToken,
                issuer: issuerToken,
                subject: claimsIdentity,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(expireTime)),
                signingCredentials: signingCredentials);

            var jwtTokenString = tokenHandler.WriteToken(jwtSecurityToken);

            return jwtTokenString;
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                var symmetricKey = Convert.FromBase64String("");

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                IEnumerable<Claim> expd = jwtToken.Claims;

                foreach (var i in expd)
                {
                    if (i.Type == "exp")
                    {
                        if (DateTime.UtcNow > ConvertTimespan(uint.Parse(i.Value)))
                            return null;
                        break;
                    }
                }

                return principal;
            }

            catch (Exception)
            {
                return null;
            }
        }

        public static DateTime ConvertTimespan(uint timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(timestamp);
        }
    }
}