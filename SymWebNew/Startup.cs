using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SymVATWebUI.Startup))]
namespace SymVATWebUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
