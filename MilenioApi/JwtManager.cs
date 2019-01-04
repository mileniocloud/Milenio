﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using MilenioApi.DAO;
using System.Web.Script.Serialization;

namespace WebApi.Jwt
{
    public static class JwtManager
    {
        /// <summary>
        /// Use the below code to generate symmetric Secret Key
        ///     var hmac = new HMACSHA256();
        ///     var key = Convert.ToBase64String(hmac.Key);
        /// </summary>
        private const string Secret = "db3OIsj+BXE9NZDy0t8W3TcNekrF+2d/1sFnWG4HnV8TZY30iTOdtVWJG8abWvB1GlOgJuQZdcF2Luqm/hccMw==";

        public static string GenerateToken(string login, List<Entidad> entidades, string userid, List<Rol> roles)
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();


            var listroles = new JavaScriptSerializer().Serialize(roles);
           // string listroles = JsonConvert.SerializeObject(roles);
            string listentidades = JsonConvert.SerializeObject(entidades);

            var now = DateTime.UtcNow;
            int expireMinutes = int.Parse(System.Configuration.ConfigurationManager.AppSettings["tokentime"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, login),
                            new Claim(ClaimTypes.NameIdentifier, userid),
                            new Claim(ClaimTypes.PrimaryGroupSid, listentidades),
                            new Claim(ClaimTypes.Role, listroles)
                        }),
                Expires = now.AddMinutes(expireMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }

        public static string GenerateInvalidToken(string username, int expireMinutes = 2)
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, username)
                        }),

                Expires = now.AddMinutes(expireMinutes),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                var symmetricKey = Convert.FromBase64String(Secret);

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