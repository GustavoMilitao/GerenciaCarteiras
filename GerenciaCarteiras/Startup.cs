using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GerenciaCarteiras.Startup))]
namespace GerenciaCarteiras
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
