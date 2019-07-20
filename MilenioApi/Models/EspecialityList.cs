using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MilenioApi.Models
{
    public class EspecialityList: ComboModel
    {
        public List<ComboModel> specialities = new List<ComboModel>();
        List<ComboModel> lista = new List<ComboModel>();
    }
}