//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MilenioApi.DAO
{
    using System;
    using System.Collections.Generic;
    
    public partial class Licencia
    {
        public System.Guid Codigo_Id { get; set; }
        public Nullable<System.Guid> Entidad_Id { get; set; }
        public int NumeroLicencia { get; set; }
        public System.DateTime FiniVigencia { get; set; }
        public System.DateTime FfinVigencia { get; set; }
        public Nullable<bool> Estado { get; set; }
        public string CostoLicencia { get; set; }
        public Nullable<System.DateTime> Created_At { get; set; }
        public Nullable<System.DateTime> Updated_At { get; set; }
        public bool IsTest { get; set; }
    
        public virtual Entidad Entidad { get; set; }
    }
}
