using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace MilenioApi.Models
{
    public class UsuarioModel: Basic
    {
        public Guid Id_Usuario { get; set; }
        [Required]
        public string Id_Tipo_Identificacion { get; set; }
        [Required]
        public string Numero_Identificacion { get; set; }
        [Required]
        public string Nombres { get; set; }
        [Required]
        public string Primer_Apellido { get; set; }
        [Required]
        public string Segundo_Apellido { get; set; }
        [Required]
        public string Sexo { get; set; }
        [Required]
        public DateTime Fecha_Nacimiento { get; set; }
        public String Foto { get; set; }
        public string Estado_Civil { get; set; }
        public string Tipo_Sangre { get; set; }
        [Required]
        public int Poblado_Id { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        [Required]
        public DateTime? Fecha_Contratacion { get; set; }
        public string Observaciones { get; set; }
        [Required]
        public string Tipo_Vinculacion { get; set; }
        [Required]
        public bool Presta_Servicio { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public bool Acepta_ABEAS { get; set; }
        public String Foto_ABEAS { get; set; }

        public int Acepta_ABEAS_Int { get; set; }

        public List<ComboModel> Roles = new List<ComboModel>();

        public int Estado { get; set; }

        [Required]
        public int Presta_Servicio_Int { get; set; }
        
        public int Id_Municipio { get; set; }
        public int Id_Departamento { get; set; }        
        public Guid Id_Tipo_Profesional { get; set; }
        public string Registro_Profesional { get; set; }
        public Guid Id_Rol { get; set; }

        public string List_Roles { get; set; }
    }
}