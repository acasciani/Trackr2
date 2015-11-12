using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Trackr.Startup))]
namespace Trackr
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
