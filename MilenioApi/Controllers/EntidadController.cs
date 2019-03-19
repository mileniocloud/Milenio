﻿using MilenioApi.Action;
using MilenioApi.Models;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/Entidad")]
    public class EntidadController : ApiController
    {
        aUtilities ut = new aUtilities();

        [HttpPost]
        [AllowAnonymous]
        [Route("GetEntidades")]
        public HttpResponseMessage GetEntidades(EntidadModel t)
        {
            aEntidad s = new aEntidad();
            return ut.ReturnResponse(s.GetEntidades(t));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("GetEntidadesEdit")]
        public HttpResponseMessage GetEntidadesEdit(EntidadModel t)
        {
            aEntidad s = new aEntidad();
            return ut.ReturnResponse(s.GetEntidadesEdit(t));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("CreateEntidad")]
        public HttpResponseMessage CreateEntidad(EntidadModel t)
        {
            aEntidad s = new aEntidad();
            return ut.ReturnResponse(s.CreateEntidad(t));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("EditEntidad")]
        public HttpResponseMessage EditEntidad(EntidadModel t)
        {
            aEntidad s = new aEntidad();
            return ut.ReturnResponse(s.EditEntidad(t));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("CreateEspecialidadEntidad")]
        public HttpResponseMessage CreateEspecialidadEntidad(EntidadModel t)
        {
            aEntidad s = new aEntidad();
            return ut.ReturnResponse(s.CreateEspecialidadEntidad(t));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("EditEspecialidadEntidad")]
        public HttpResponseMessage EditEspecialidadEntidad(EntidadModel t)
        {
            aEntidad s = new aEntidad();
            return ut.ReturnResponse(s.EditEspecialidadEntidad(t));
        }
    }
}
