
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
    
public partial class Curso
{

    public System.Guid Codigo_Id { get; set; }

    public int Nivel { get; set; }

    public string Nombre { get; set; }

    public Nullable<System.Guid> Aula_Id { get; set; }

    public System.DateTime Created_At { get; set; }

    public Nullable<System.DateTime> Updated_At { get; set; }



    public virtual Aula Aula { get; set; }

}

}
