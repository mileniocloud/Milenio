using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using MilenioApi.Action;
using MilenioApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MilenioApi.Controllers
{
    public class TokenValidationHandler
    {
        public ClaimsPrincipal getprincipal(string token)
        {
            var secretKey = ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
            var audienceToken = ConfigurationManager.AppSettings["JWT_AUDIENCE_TOKEN"];
            var issuerToken = ConfigurationManager.AppSettings["JWT_ISSUER_TOKEN"];
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secretKey));

            SecurityToken securityToken;
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            TokenValidationParameters validationParameters = new TokenValidationParameters()
            {
                ValidAudience = audienceToken,
                ValidIssuer = issuerToken,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                LifetimeValidator = this.LifetimeValidator,
                IssuerSigningKey = securityKey
            };
           return tokenHandler.ValidateToken(token, validationParameters, out securityToken);
            
        }
               
        public bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if (expires != null)
            {
                if (DateTime.UtcNow < expires) return true;
            }
            return false;
        }


        internal class TokenValidateHandler : DelegatingHandler
        {
            private static bool TryRetrieveToken(HttpRequestMessage request, out string token)
            {
                token = null;
                IEnumerable<string> authzHeaders;
                if (!request.Headers.TryGetValues("Authorization", out authzHeaders) || authzHeaders.Count() > 1)
                {
                    return false;
                }
                var bearerToken = authzHeaders.ElementAt(0);
                token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
                return true;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {               
                Response rp = new Response();
                aUtilities autil = new aUtilities();

                string token;

                // determine whether a jwt exists or not
                if (!TryRetrieveToken(request, out token))
                {                    
                    return base.SendAsync(request, cancellationToken);
                }

                try
                {
                    var secretKey = ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
                    var audienceToken = ConfigurationManager.AppSettings["JWT_AUDIENCE_TOKEN"];
                    var issuerToken = ConfigurationManager.AppSettings["JWT_ISSUER_TOKEN"];
                    var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secretKey));
                    IdentityModelEventSource.ShowPII = true;

                    SecurityToken securityToken;
                    var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                    TokenValidationParameters validationParameters = new TokenValidationParameters()
                    {
                        ValidAudience = audienceToken,
                        ValidIssuer = issuerToken,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,                        
                        LifetimeValidator = this.LifetimeValidator,
                        IssuerSigningKey = securityKey
                    };

                    // Extract and assign Current Principal and user
                    Thread.CurrentPrincipal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
                    HttpContext.Current.User = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                    return base.SendAsync(request, cancellationToken);
                }
                catch (SecurityTokenValidationException ex)
                {                    

                    if (ex.ToString().Contains("Lifetime validation failed"))
                    {
                        autil.MensajeRetorno(ref rp, 1, string.Empty, null, HttpStatusCode.Redirect);                        
                    }
                    else
                    {
                        autil.MensajeRetorno(ref rp, 1, string.Empty, null, HttpStatusCode.Unauthorized);                       
                    }

                }
                catch (Exception ex)
                {                    
                    autil.MensajeRetorno(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);                    
                }

                return Task<HttpResponseMessage>.Factory.StartNew(() => autil.ReturnResponse(rp));
            }

            public bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
            {
                if (expires != null)
                {
                    if (DateTime.UtcNow < expires) return true;
                }
                return false;
            }

        }
    }
}