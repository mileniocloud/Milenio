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
        [DataType(DataType.DateTime)]
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
        [DataType(DataType.DateTime)]
        public DateTime Fecha_Contratacion { get; set; }
        [Display(Name = "observation")]
        [JsonProperty("observation")]
        public string Observaciones { get; set; }

        [Required]
        [Display(Name = "linktype")]
        [JsonProperty("linktype")]
        public Guid Id_Tipo_Vinculacion { get; set; }

        [Required]
        [Display(Name = "serviceprovider")]
        [JsonProperty("serviceprovider")]
        public bool Presta_Servicio { get; set; }
        [Required]
        [Display(Name = "login")]
        [JsonProperty("login")]
        public string Login { get; set; }
        [Required]
        [Display(Name = "password")]
        [JsonProperty("password")]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "email")]
        [JsonProperty("email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "habeas")]
        [JsonProperty("habeas")]
        public bool Acepta_ABEAS { get; set; }
        [Display(Name = "photohabeas")]
        [JsonProperty("photohabeas")]
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

        [Required]
        [Display(Name = "typeprofessional")]
        [JsonProperty("typeprofessional")]
        public Guid Id_Tipo_Profesional { get; set; }
        [Display(Name = "registryprofessional")]
        [JsonProperty("registryprofessional")]
        public string Registro_Profesional { get; set; }
        [Display(Name = "idrole")]
        [JsonProperty("idrole")]
        public Guid Id_Rol { get; set; }

        [Display(Name = "rolelist")]
        [JsonProperty("rolelist")]
        public List<ComboListModel> List_Roles = new List<ComboListModel>();
    }
}