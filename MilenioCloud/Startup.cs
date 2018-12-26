using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MilenioCloud.Startup))]
namespace MilenioCloud
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
