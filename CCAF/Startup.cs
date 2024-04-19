using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CCAF.Startup))]
namespace CCAF
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
