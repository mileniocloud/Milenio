using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using WebApi.Jwt;
using WebApi.Jwt.Filters;
using MilenioApi.Models;
using System.IO;


namespace MilenioApi.Controllers
{
    [RoutePrefix("api/Token")]
    public class TokenController : ApiController
    {
        public ClaimsPrincipal ValidateToken(string token)
        {
            return JwtManager.GetPrincipal(token);
        }
    }
}