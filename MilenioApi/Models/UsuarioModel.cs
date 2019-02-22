using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace MilenioApi.Models
{
    public class UsuarioModel
    {
        public Guid Id_Usuario { get; set; }
        public string Id_Tipo_Identificacion { get; set; }
        public string Numero_Identificacion { get; set; }
        public string Nombres { get; set; }
        public string Primer_Apellido { get; set; }
        public string Segundo_Apellido { get; set; }
        public string Sexo { get; set; }
        public DateTime Fecha_Nacimiento { get; set; }
        public String Foto { get; set; }
        public string Estado_Civil { get; set; }
        public string TipoSangre { get; set; }
        public int Poblado_Id { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public DateTime Fecha_Contratacion { get; set; }
        public string Observaciones { get; set; }
        public Guid Id_Tipo_Vinculacion { get; set; }
        public bool Presta_Servicio { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public bool Acepta_ABEAS { get; set; }
        public String Foto_ABEAS { get; set; }

        public int Presta_Servicio_Int { get; set; }
        public int Acepta_ABEAS_Int { get; set; }
        public int Id_Municipio { get; set; }
        public int Id_Departamento { get; set; }
    }
}