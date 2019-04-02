using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using MilenioApi.Action;
using MilenioApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;

namespace MilenioApi.Controllers
{
    public class TokenValidationHandler
    {
        public ClaimsPrincipal getprincipal(string token)
        {
            token = Decrypt(token);

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

        public string GenerateToken(string login, string userid, List<string> roles, Guid? entidad_id)
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
            if (roles != null)
            {
                foreach (var r in roles)
                {
                    Claim cc = new Claim(ClaimTypes.Role, r);
                    claimsIdentity.AddClaim(cc);
                }
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

            return Encrypt(jwtTokenString);
        }

        private string Encrypt(string token)
        {
            var secretKey = ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
            string PasswordHash = "P@@Sw0rd";
            string SaltKey = secretKey;
            string VIKey = "@1B2c3D4e5F6g7H8";

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(token);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);

        }

        public static string Decrypt(string encryptedText)
        {
            var secretKey = ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
            string PasswordHash = "P@@Sw0rd";
            string SaltKey = secretKey;
            string VIKey = "@1B2c3D4e5F6g7H8";

            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }

        public static DateTime ConvertTimespan(uint timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(timestamp);
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
                token = Decrypt(bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken);

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
                        LoginModel lm = new LoginModel();
                        lm.token = token;
                        aSeguridad sg = new aSeguridad();
                        ClaimsPrincipal cp = getprincipalnotime(token);
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());
                        sg.TimeLogOff(usuario);

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

            private ClaimsPrincipal getprincipalnotime(string token)
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
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey
                };
                return tokenHandler.ValidateToken(token, validationParameters, out securityToken);

            }
        }
    }
}