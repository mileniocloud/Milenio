using System.Collections.Generic;

namespace MilenioApi.Models
{
    public class DashboardModel: Basic
    {
        public string foto { get; set; }
        public List<MenuModel> menu = new List<MenuModel>();
    }
}