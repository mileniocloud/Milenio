using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace MilenioApi.Models
{
    public class UserModel : Basic
    {
        [Display(Name = "iduser")]
        [JsonProperty("iduser")]
        public Guid Id_Usuario { get; set; }
        [Required]
        [Display(Name = "typedocument")]
        [JsonProperty("typedocument")]
        public string Id_Tipo_Identificacion { get; set; }
        [Required]
        [Display(Name = "document")]
        [JsonProperty("document")]
        public string Numero_Identificacion { get; set; }
        [Required]
        [Display(Name = "fullname")]
        [JsonProperty("fullname")]
        public string Nombres { get; set; }
        [Required]
        [Display(Name = "firstlastname")]
        [JsonProperty("firstlastname")]
        public string Primer_Apellido { get; set; }
        [Required]
        [Display(Name = "secondlastname")]
        [JsonProperty("secondlastname")]
        public string Segundo_Apellido { get; set; }
        [Required]
        [Display(Name = "gender")]
        [JsonProperty("gender")]
        public string Sexo { get; set; }
        [Required]
        [Display(Name = "birthdate")]
        [JsonProperty("birthdate")]
        public DateTime Fecha_Nacimiento { get; set; }
        [Display(Name = "photo")]
        [JsonProperty("photo")]
        public String Foto { get; set; }
        [Display(Name = "civilstatus")]
        [JsonProperty("civilstatus")]
        public string Estado_Civil { get; set; }
        [Display(Name = "bloodtype")]
        [JsonProperty("bloodtype")]
        public string Tipo_Sangre { get; set; }
        [Required]
        [Display(Name = "neighborhood")]
        [JsonProperty("neighborhood")]
        public int Poblado_Id { get; set; }
        [Display(Name = "address")]
        [JsonProperty("address")]
        public string Direccion { get; set; }
        [Display(Name = "telephone")]
        [JsonProperty("telephone")]
        public string Telefono { get; set; }
        [Required]
        [Display(Name = "dateofhire")]
        [JsonProperty("dateofhire")]
        public DateTime? Fecha_Contratacion { get; set; }
        [Display(Name = "observation")]
        [JsonProperty("observation")]
        public string Observaciones { get; set; }
        [Required]
        [Display(Name = "linktype")]
        [JsonProperty("linktype")]
        public string Tipo_Vinculacion { get; set; }
        [Required]
        [Display(Name = "isservices")]
        [JsonProperty("isservices")]
        public bool Presta_Servicio { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Display(Name = "abeas")]
        [JsonProperty("abeas")]
        public bool Acepta_ABEAS { get; set; }
        [Display(Name = "photoabeas")]
        [JsonProperty("photoabeas")]
        public String Foto_ABEAS { get; set; }
        public int Acepta_ABEAS_Int { get; set; }

        [Display(Name = "roles")]
        [JsonProperty("roles")]
        public List<ComboModel> Roles = new List<ComboModel>();

        [Display(Name = "status")]
        [JsonProperty("status")]
        public int Estado { get; set; }

        [Required]
        public int Presta_Servicio_Int { get; set; }

        [Display(Name = "municipality")]
        [JsonProperty("municipality")]
        public int Id_Municipio { get; set; }
        [Display(Name = "departament")]
        [JsonProperty("departament")]
        public int Id_Departamento { get; set; }
        [Display(Name = "typeprofessional")]
        [JsonProperty("typeprofessional")]
        public Guid Id_Tipo_Profesional { get; set; }
        [Display(Name = "registryprofessional")]
        [JsonProperty("registryprofessional")]
        public string Registro_Profesional { get; set; }
        [Display(Name = "idrole")]
        [JsonProperty("idrole")]
        public Guid Id_Rol { get; set; }

        public string List_Roles { get; set; }
    }
}